using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;
using Zenject;
using PlayerModel;
using GameModel;
using PlayerView;
using StateView;
using TriggerView;
using StageObject;
using SoundManager;
using static StateType;
using static SEType;

namespace PlayerPresenter
{
    public class PlayerPresenter : MonoBehaviour
    {
        #region//インスペクターから設定
        [SerializeField]
        [Header("プレイヤーの初期hpを設定")]
        int _initialHp = 3;

        [SerializeField]
        [Header("プレイヤーの武器の攻撃力を設定")]
        int _initialPower = 0;

        [SerializeField]
        [Header("プレイヤーの移動速度")]
        float _speed = 10.0f;

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
        [Header("装備武器を設定")]
        PlayerWeapon _playerWeapon;
        #endregion

        #region//フィールド
        ActionView _actionView;//プレイヤーのアクション用スクリプト
        WaitView _waitView;//待機状態のスクリプト
        RunView _runView;//移動状態のスクリプト
        DownView _downView;//ダウン状態のスクリプト
        DeadView _deadView;//デッド状態のスクリプト
        AttackView _attackView;//攻撃状態のスクリプト
        JoyView _joyView;//喜び状態のスクリプト
        TriggerView.TriggerView _triggerView;//接触判定スクリプト
        CollisionView _collisionView;//衝突判定スクリプト
        InputView _inputView;//プレイヤーの入力取得スクリプト
        Rigidbody _rigidBody;
        Animator _animator;
        ObservableStateMachineTrigger _animTrigger;
        IDirectionModel _directionModel;
        IWeaponModel _weaponModel;
        IHpModel _hpModel;
        IScoreModel _scoreModel;
        IPointModel _pointModel;
        ISoundManager _soundManager;
        bool _isBlink;//点滅状態か
        #endregion

        #region//プロパティ
        #endregion

        [Inject]
        public void Construct(
            IWeaponModel weapon,
            IHpModel hp,
            IScoreModel score,
            IPointModel point,
            IDirectionModel direction,
            ISoundManager soundManager
        )
        {
            _weaponModel = weapon;
            _hpModel = hp;
            _scoreModel = score;
            _pointModel = point;
            _directionModel = direction;
            _soundManager = soundManager;
        }

        /// <summary>
        /// プレハブのインスタンス直後の処理
        /// </summary>
        public void ManualAwake()
        {
            _actionView = GetComponent<ActionView>();
            _waitView = GetComponent<WaitView>();
            _runView = GetComponent<RunView>();
            _downView = GetComponent<DownView>();
            _deadView = GetComponent<DeadView>();
            _attackView = GetComponent<AttackView>();
            _joyView = GetComponent<JoyView>();
            _triggerView = GetComponent<TriggerView.TriggerView>();
            _collisionView = GetComponent<CollisionView>();
            _inputView = GetComponent<InputView>();
            _rigidBody = GetComponent<Rigidbody>();
            _animator = GetComponent<Animator>();
            _animTrigger = _animator.GetBehaviour<ObservableStateMachineTrigger>();
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        public void Initialize()
        {
            InitializeModel();
            InitializeView();
            Bind();
        }

        /// <summary>
        /// モデルの初期化を行います
        /// </summary>
        void InitializeModel()
        {
            _weaponModel.SetPower(_initialPower);
            _hpModel.SetHp(_initialHp);
        }

        /// <summary>
        /// ビューの初期化を行います
        /// </summary>
        void InitializeView()
        {
            _runView.DelAction = Run;
            _actionView.State.Value = _waitView;
            _playerWeapon.Initialize();
        }

        /// <summary>
        /// リセットします
        /// </summary>
        public void ResetData()
        {
            _actionView.State.Value = _waitView;
            InitializeModel();
        }

        /// <summary>
        /// modelとviewの監視、処理
        /// </summary>
        void Bind()
        {
            //modelの監視
            _hpModel.Hp.Subscribe(hp => _hpView.SetHpGauge(hp)).AddTo(this);

            //trigger, collisionの取得
            _triggerView.OnTriggerEnter()
                .Where(_ => _directionModel.CanGame())
                .Subscribe(collider => CheckColliderEnter(collider))
                .AddTo(this);

            _collisionView.OnCollisionEnter()
                .Where(_ => _directionModel.CanGame())
                .Subscribe(collision => CheckCollisionEnter(collision))
                .AddTo(this);

            //viewの監視
            //状態の監視
            _actionView.State
                .Where(x => x != null)
                .Subscribe(x => {
                    _actionView.ChangeState(x.State);
                })
                .AddTo(this);

            //入力の監視
            _inputView.InputDirection
                .Where(_ => _directionModel.CanGame())
                .Subscribe(input =>
                {
                    //攻撃中に入力した場合攻撃モーションを終了する
                    if (_actionView.HasActionBy(ATTACK))
                        _playerWeapon.EndMotion();

                    ChangeStateByInput(input);
                }
                )
                .AddTo(this);

            //攻撃入力
            _inputView.IsFired
                .Where(x => (x == true)
                && _directionModel.CanGame()
                && IsControllableState())
                .Subscribe(_ => ChangeAttack())
                .AddTo(this);

            //アニメーションの監視
            _animTrigger.OnStateUpdateAsObservable()
                .Where(s => s.StateInfo.IsName("Attack")
                || s.StateInfo.IsName("Down"))
                .Where(s => s.StateInfo.normalizedTime >= s.StateInfo.length)
                .Subscribe(_ =>
                {
                    EndMotion();
                    _actionView.State.Value = _waitView;
                })
                .AddTo(this);
        }

        void EndMotion()
        {
            //攻撃前の方向に戻します
            _playerWeapon.EndMotion();
        }

        /// <summary>
        /// 操作可能な状態か
        /// </summary>
        bool IsControllableState()
        {
            return (_actionView.HasActionBy(RUN)
                || _actionView.HasActionBy(WAIT));
        }

        /// <summary>
        /// fixedUpdate処理
        /// </summary>
        public void ManualFixedUpdate()
        {
            _actionView.Action();
        }

        /// <summary>
        /// 接触時に確認します
        /// </summary>
        /// <param name="collider"></param>
        void CheckColliderEnter(Collider collider)
        {
            GetPointItemBy(collider);
            ReceiveDamageBy(collider);
        }

        /// <summary>
        /// 衝突時に確認します
        /// </summary>
        void CheckCollisionEnter(Collision collision)
        {
            ReceiveDamageBy(collision.collider);
        }

        /// <summary>
        /// ポイントアイテムの取得を試みます
        /// </summary>
        void GetPointItemBy(Collider collider)
        {
            if (collider.TryGetComponent(out IPointItem pointItem))
            {
                _soundManager.PlaySE(POINT_GET);
                _pointModel.AddPoint(pointItem.Point);
                _scoreModel.AddScore(pointItem.Score);
                pointItem.Destroy();
            }
        }

        /// <summary>
        /// ダメージを受けるか確認します
        /// </summary>
        void ReceiveDamageBy(Collider collider)
        {
            if (_isBlink) return;
            if (_actionView.HasActionBy(ATTACK)) return;//攻撃中はダメージを受けない
            if (collider.TryGetComponent(out IDamager damager))
            {
                _soundManager.PlaySE(DAMAGED);
                _hpModel.ReduceHp(damager.Damage);
                ChangeStateByDamage();
                KnockBack(collider?.gameObject);
            }
        }

        /// <summary>
        /// hpを増やします
        /// </summary>
        /// <param name="hp"></param>
        public void AddHp(int hp)
        {
            //hpは初期値以上は増えないようにする
            if (_hpModel.Hp.Value >= _initialHp) return;
            _soundManager.PlaySE(HP_UP);
            _hpModel.AddHp(hp);
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
            _playerWeapon.StartMotion();
            _actionView.State.Value = _attackView;
        }

        /// <summary>
        /// ダメージによってプレイヤーの状態を切り替えます
        /// </summary>
        void ChangeStateByDamage()
        {
            if (_hpModel.Hp.Value > 0)
                ChangeDown();
            else ChangeDead();
        }

        void ChangeDown()
        {
            _actionView.State.Value = _downView;
            PlayerBlinks().Forget();//点滅処理
        }

        public void ChangeDead()
        {
            _actionView.State.Value = _deadView;
            _directionModel.SetIsGameOver(true);
        }

        public void ChangeJoy()
        {
            _actionView.State.Value = _joyView;
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
            knockBackDirection.y = 0;//Y方向には飛ばないようにする
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
        /// 走ります
        /// </summary>
        void Run()
        {
            if (_directionModel.CanGame() == false) return;

            Vector2 input = _inputView.InputDirection.Value;
            Move(input);
            Rotation(input);
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
    }
}