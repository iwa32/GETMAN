using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;
using Zenject;
using PlayerModel;
using PlayerView;
using static PlayerState;

namespace PlayerPresenter
{
    public class PlayerPresenter : MonoBehaviour
    {
        #region//インスペクターから設定
        [SerializeField]
        [Header("プレイヤーの移動速度")]
        float _speed = 10.0f;

        [SerializeField]
        [Header("Hpを取得するスコアライン")]
        int _scoreLineToGetHp = 100;

        [SerializeField]
        [Header("次は〇倍後のスコアラインでHpを取得します")]
        int nextMagnification = 5;

        [SerializeField]
        [Header("プレイヤーの点滅時間")]
        float _blinkTime = 3.0f;

        [SerializeField]
        [Header("ノックバック時の飛ぶ威力")]
        float _knockBackPower = 10.0f;

        [SerializeField]
        [Header("武器アイコンのUIを設定")]
        WeaponView _weaponView;

        [SerializeField]
        [Header("HPのUIを設定")]
        HpView _hpView;

        [SerializeField]
        [Header("スコアのUIを設定")]
        ScoreView _scoreView;

        [SerializeField]
        [Header("獲得ポイントのUIを設定")]
        PointView _pointView;

        [SerializeField]
        [Header("プレイヤーのアクション用スクリプトを設定")]
        ActionView _actionView;

        [SerializeField]
        [Header("待機状態のスクリプトを設定")]
        WaitView _waitView;

        [SerializeField]
        [Header("移動状態のスクリプトを設定")]
        RunView _runView;

        [SerializeField]
        [Header("ダウン状態のスクリプトを設定")]
        DownView _downView;

        [SerializeField]
        [Header("デッド状態のスクリプトを設定")]
        DeadView _deadView;

        [SerializeField]
        [Header("攻撃状態のスクリプトを設定")]
        AttackView _attackView;

        [SerializeField]
        [Header("プレイヤーの入力取得スクリプトを設定")]
        InputView _inputView;

        [SerializeField]
        [Header("接触判定スクリプトを設定")]
        TriggerView _triggerView;

        [SerializeField]
        [Header("衝突判定スクリプトを設定")]
        CollisionView _collisionView;
        #endregion

        #region//フィールド
        Rigidbody _rigidBody;
        Animator _animator;
        ObservableStateMachineTrigger _animTrigger;
        IWeaponModel _weaponModel;
        IHpModel _hpModel;
        IScoreModel _scoreModel;
        IPointModel _pointModel;
        IStateModel _stateModel;
        bool _isBlink;//点滅状態か
        #endregion

        [Inject]
        public void Construct(
            IWeaponModel weapon,
            IHpModel hp,
            IScoreModel score,
            IPointModel point,
            IStateModel state
        )
        {
            _weaponModel = weapon;
            _hpModel = hp;
            _scoreModel = score;
            _pointModel = point;
            _stateModel = state;
        }

        /// <summary>
        /// プレハブのインスタンス直後の処理
        /// </summary>
        public void ManualAwake()
        {
            _rigidBody = GetComponent<Rigidbody>();
            _animator = GetComponent<Animator>();
            _animTrigger = _animator.GetBehaviour<ObservableStateMachineTrigger>();
        }

        public void Initialize()
        {
            _waitView.DelAction = Wait;
            _runView.DelAction = Run;
            _downView.DelAction = Down;
            _deadView.DelAction = Dead;
            _attackView.DelAction = Attack;
            _actionView.State.Value = _waitView;
            Bind();
        }

        /// <summary>
        /// modelとviewの監視、処理
        /// </summary>
        void Bind()
        {
            //modelの監視
            _hpModel.Hp.Subscribe(hp => _hpView.SetHpGauge(hp));
            _scoreModel.Score.Subscribe(score => CheckScore(score));
            _pointModel.Point.Subscribe(point => _pointView.SetPointGauge(point));

            //trigger, collisionの取得
            _triggerView.OnTrigger().Subscribe(collider => CheckCollider(collider));
            _collisionView.OnCollision().Subscribe(collision => CheckCollision(collision));

            //viewの監視
            _actionView.State
                .Where(x => x != null)
                .Subscribe(x => {
                    _actionView.ChangeState(x.State);
            });

            //入力の監視
            //WAITとRUNのみ入力を受け付けます
            _inputView.InputDirection
                .Where(_ => (_actionView.State.Value.State == RUN
                || _actionView.State.Value.State == WAIT))
                .Subscribe(input => ChangeStateByInput(input));
            //攻撃入力
            _inputView.IsFired
                .Where(x => (x == true)
                && (_actionView.State.Value.State == RUN
                || _actionView.State.Value.State == WAIT))
                .Subscribe(x => {
                    ChangeAttack();
                });

            //アニメーションの監視
            _animTrigger.OnStateUpdateAsObservable()
                .Where(s => s.StateInfo.IsName("Attack")
                || s.StateInfo.IsName("Down"))
                .Where(s => s.StateInfo.normalizedTime >= 1)
                .Subscribe(_ =>
                {
                    _actionView.State.Value = _waitView;
                })
                .AddTo(this);
        }

        /// <summary>
        /// fixedUpdate処理
        /// </summary>
        public void ManualFixedUpdate()
        {
            _actionView.Action();
        }

        /// <summary>
        /// スコアを監視する
        /// </summary>
        void CheckScore(int score)
        {
            CheckScoreToGetHp(score);
            _scoreView.SetScore(score);
        }

        /// <summary>
        /// Scoreを決められた数取得するとHPがアップします
        /// </summary>
        /// <param name="score"></param>
        void CheckScoreToGetHp(int score)
        {
            if (score <= 0) return;
            if (score % _scoreLineToGetHp == 0)
            {
                _hpModel.AddHp(1);
                _scoreLineToGetHp *= nextMagnification;
            }
        }

        /// <summary>
        /// 接触したコライダーを確認します
        /// </summary>
        /// <param name="collider"></param>
        void CheckCollider(Collider collider)
        {
            TryGetPointItem(collider);
            //TryReceiveDamage(collider);todo ダメージ床用
        }

        /// <summary>
        /// 衝突を確認します
        /// </summary>
        void CheckCollision(Collision collision)
        {
            TryCheckEnemyCollision(collision);
        }

        /// <summary>
        /// 敵の接触の確認を試みます
        /// </summary>
        void TryCheckEnemyCollision(Collision collision)
        {
            if (collision.gameObject.TryGetComponent(out IEnemy enemy))
            {
                TryReceiveDamage(collision.collider);
            }
        }

        /// <summary>
        /// ポイントアイテムの取得を試みます
        /// </summary>
        void TryGetPointItem(Collider collider)
        {
            if (collider.TryGetComponent(out IPointItem pointItem))
            {
                _pointModel.AddPoint(pointItem.Point);
                _scoreModel.AddScore(pointItem.Score);
                pointItem.Destroy();
            }
        }

        /// <summary>
        /// ダメージを受けるか確認します
        /// </summary>
        void TryReceiveDamage(Collider collider)
        {
            if (_isBlink) return;
            if (collider.TryGetComponent(out IAttacker attacker))
            {
                _hpModel.ReduceHp(attacker.Power);
                ChangeStateByDamage();
                KnockBack(collider?.gameObject);
            }
        }

        /// <summary>
        /// 入力の有無でプレイヤーの状態を切り替えます
        /// </summary>
        /// <param name="input"></param>
        void ChangeStateByInput(Vector2 input)
        {
            if (input.magnitude != 0)
                _actionView.State.Value = _runView;
            else
                _actionView.State.Value = _waitView;
        }

        /// <summary>
        /// 攻撃状態に切り替えます
        /// </summary>
        void ChangeAttack()
        {
            //if 武器があれば攻撃する

            _actionView.State.Value = _attackView;
        }

        /// <summary>
        /// ダメージによってプレイヤーの状態を切り替えます
        /// </summary>
        void ChangeStateByDamage()
        {
            if (_hpModel.Hp.Value > 0)
                ChangeDown();
            else
                _actionView.State.Value = _deadView;
        }

        /// <summary>
        /// 走ります
        /// </summary>
        void Run()
        {
            Vector2 input = _inputView.InputDirection.Value;
            Move(input);
            Rotation(input);
        }

        void ChangeDown()
        {
            _actionView.State.Value = _downView;
            //点滅処理
            PlayerBlinks().Forget();
        }

        /// <summary>
        /// ノックバックします
        /// </summary>
        void KnockBack(GameObject target)
        {
            //ノックバック方向を取得
            Vector3 knockBackDirection = (transform.position - target.transform.position).normalized;

            //速度ベクトルをリセット
            _rigidBody.velocity = Vector3.zero;
            _rigidBody.AddForce(knockBackDirection * _knockBackPower, ForceMode.VelocityChange);
        }

        /// <summary>
        /// プレイヤーの点滅
        /// </summary>
        async UniTask PlayerBlinks()
        {
            bool isActive = false;
            float elapsedBlinkTime = 0.0f;

            _isBlink = true;
            while (elapsedBlinkTime <= _blinkTime)
            {
                SetActiveToAllChild(isActive);
                isActive = !isActive;
                await UniTask.Delay(TimeSpan.FromSeconds(0.2f));
                elapsedBlinkTime += 0.2f;
            }

            SetActiveToAllChild(true);
            _isBlink = false;
        }

        /// <summary>
        /// 子要素を全てアクティブ・非アクティブにする
        /// </summary>
        /// <param name="isActive"></param>
        void SetActiveToAllChild(bool isActive)
        {
            foreach (Transform child in gameObject.transform)
            {
                child.gameObject.SetActive(isActive);
            }
        }

        /// <summary>
        /// ダウンします
        /// </summary>
        void Down()
        {
        }

        /// <summary>
        /// 移動します
        /// </summary>
        /// <param name="input"></param>
        void Move(Vector2 input)
        {
            //入力があった場合
            if (input != Vector2.zero)
            {
                Vector3 movePos = new Vector3(input.x, 0, input.y);
                _rigidBody.velocity = movePos * _speed;
            }
        }

        /// <summary>
        /// 回転します
        /// </summary>
        /// <param name="input"></param>
        void Rotation(Vector2 input)
        {
            _rigidBody.rotation = Quaternion.LookRotation(new Vector3(input.x, 0, input.y));
        }

        /// <summary>
        /// 待機状態
        /// </summary>
        void Wait()
        {
            
        }

        /// <summary>
        /// やられてしまった
        /// </summary>
        void Dead()
        {
            Debug.Log("dead");
        }

        /// <summary>
        /// 攻撃を行います
        /// </summary>
        void Attack()
        {
            Debug.Log("攻撃");
        }
    }
}