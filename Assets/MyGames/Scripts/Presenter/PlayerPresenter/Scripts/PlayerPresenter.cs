using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Zenject;
using PlayerModel;
using PlayerView;
using static PlayerState;

namespace PlayerPresenter
{
    public class PlayerPresenter : MonoBehaviour
    {
        #region//フィールド
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
        [Header("プレイヤーの入力取得スクリプトを設定")]
        InputView _inputView;

        [SerializeField]
        [Header("プレイヤーの移動速度を設定")]
        float _speed = 10.0f;

        [SerializeField]
        [Header("Hpを取得するスコアラインを設定")]
        int _scoreLineToGetHp = 100;

        [SerializeField]
        [Header("次は〇倍後のスコアラインでHpを取得します")]
        int nextMagnification = 5;

        [SerializeField]
        [Header("接触判定スクリプトを設定")]
        TriggerView _triggerView;

        [SerializeField]
        [Header("衝突判定スクリプトを設定")]
        CollisionView _collisionView;
        #endregion

        #region//プロパティ
        Rigidbody _rigidBody;
        Animator _animator;
        ObservableStateMachineTrigger _animTrigger;
        IWeaponModel _weaponModel;
        IHpModel _hpModel;
        IScoreModel _scoreModel;
        IPointModel _pointModel;
        IStateModel _stateModel;
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

        void Awake()
        {
            _rigidBody = GetComponent<Rigidbody>();
            _animator = GetComponent<Animator>();
            _animTrigger = _animator.GetBehaviour<ObservableStateMachineTrigger>();
        }

        void Start()
        {
            Initialize();
        }

        void Initialize()
        {
            _waitView.DelAction = Wait;
            _runView.DelAction = Run;
            _downView.DelAction = Down;
            _deadView.DelAction = Dead;
            _actionView.State.Value = _waitView;
            Bind();
        }

        /// <summary>
        /// 監視し、ModelとUIの紐付けを行う
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

            //WAITとRUNのみ入力を受け付けます
            _inputView.InputDirection
                .Where(_ => (_actionView.State.Value.State == RUN
                || _actionView.State.Value.State == WAIT))
                .Subscribe(input => ChangeStateByInput(input));

            //animationの監視
            _animTrigger.OnStateEnterAsObservable()
                .Where(s => s.StateInfo.IsName("Down"))
                .SkipWhile(s => s.StateInfo.normalizedTime >= 1.0f)
                .Subscribe(x => { _actionView.State.Value = _waitView; })//１秒後ダウン終了
                .AddTo(this);
        }

        void FixedUpdate()
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
            TryReceiveDamage(collider);
        }

        /// <summary>
        /// 衝突を確認します
        /// </summary>
        void CheckCollision(Collision collision)
        {
            TryReceiveDamage(collision.collider);
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
            if (collider.TryGetComponent(out IAttacker attacker))
            {
                _hpModel.ReduceHp(attacker.Power);
                ChangeStateByDamage();
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
        /// ダメージによってプレイヤーの状態を切り替えます
        /// </summary>
        void ChangeStateByDamage()
        {
            if (_hpModel.Hp.Value > 0)
                _actionView.State.Value = _downView;
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

        /// <summary>
        /// ダウンします
        /// </summary>
        void Down()
        {
            //一度だけ処理
            Debug.Log("down");
            //点滅処理
            //アニメーション終了

            //ノックバック
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
    }
}