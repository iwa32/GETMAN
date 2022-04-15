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
using Trigger;
using Collision;
using StageObject;
using SpWeaponDataList;
using SoundManager;
using static StateType;
using static SEType;
using GlobalInterface;
using NormalPlayerWeapon;
using SpPlayerWeaponInvoker;

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
        [Header("HPのUIを設定")]
        HpView _hpView;

        [SerializeField]
        [Header("SP武器表示用のUIを設定")]
        SpWeaponView _spWeaponView;

        [SerializeField]
        [Header("装備武器を設定")]
        PlayerSword _playerWeapon;

        #region//インスペクターから設定
        [SerializeField]
        [Header("SpWeaponのScritableObjectを設定")]
        SpWeaponDataList.SpWeaponDataList _spWeaponDataList;
        #endregion
        #endregion

        #region//フィールド
        ActionView _actionView;//プレイヤーのアクション用スクリプト
        WaitState _waitState;//待機状態のスクリプト
        RunState _runState;//移動状態のスクリプト
        DownState _downState;//ダウン状態のスクリプト
        DeadState _deadState;//デッド状態のスクリプト
        AttackState _attackState;//攻撃状態のスクリプト
        JoyState _joyState;//喜び状態のスクリプト
        ObservableTrigger _trigger;//接触判定スクリプト
        ObservableCollision _collision;//衝突判定スクリプト
        InputView _inputView;//プレイヤーの入力取得スクリプト
        Rigidbody _rigidBody;
        Animator _animator;
        ObservableStateMachineTrigger _animTrigger;
        ISpPlayerWeaponInvoker _currentSpWeapon;//現在取得しているSP武器を保持
        IDirectionModel _directionModel;
        IWeaponModel _weaponModel;
        IHpModel _hpModel;
        IScoreModel _scoreModel;
        IPointModel _pointModel;
        ISoundManager _soundManager;
        bool _isBlink;//点滅状態か

        [Inject]
        DiContainer container;//動的生成したデータにDIできるようにする
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
            _waitState = GetComponent<WaitState>();
            _runState = GetComponent<RunState>();
            _downState = GetComponent<DownState>();
            _deadState = GetComponent<DeadState>();
            _attackState = GetComponent<AttackState>();
            _joyState = GetComponent<JoyState>();
            _trigger = GetComponent<ObservableTrigger>();
            _collision = GetComponent<ObservableCollision>();
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
            _runState.DelAction = Run;
            _actionView.State.Value = _waitState;
        }

        /// <summary>
        /// リセットします
        /// </summary>
        public void ResetData()
        {
            _actionView.State.Value = _waitState;
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
            _trigger.OnTriggerStay()
                .Where(_ => _directionModel.CanGame())
                .Subscribe(collider => CheckTrigger(collider))
                .AddTo(this);

            _collision.OnCollisionStay()
                .Where(_ => _directionModel.CanGame())
                .Subscribe(collision => CheckCollision(collision))
                .AddTo(this);

            //viewの監視
            //状態の監視
            _actionView.State
                .Where(x => x != null)
                .Subscribe(x =>
                {
                    _actionView.ChangeState(x.State);
                })
                .AddTo(this);

            //入力の監視
            _inputView.InputDirection
                .Where(_ => _directionModel.CanGame())
                .Subscribe(input =>
                {
                    ChangeStateByInput(input);
                }
                )
                .AddTo(this);

            //攻撃入力
            //剣での攻撃
            _inputView.IsFired
                .Where(x => (x == true)
                && _directionModel.CanGame()
                && IsControllableState())
                .Subscribe(_ => ChangeAttack())
                .AddTo(this);

            //SP武器での攻撃
            _inputView.IsSpAttack
                .Where(x => (x == true)
                && _directionModel.CanGame()
                && IsControllableState()
                && _currentSpWeapon != null
                )
                .Subscribe(_ => _currentSpWeapon.Invoke())
                .AddTo(this);

            //アニメーションの監視
            _animTrigger.OnStateExitAsObservable()
                .Where(s => s.StateInfo.IsName("Attack")
                || s.StateInfo.IsName("Attack2")
                || s.StateInfo.IsName("Attack3")
                )
                .Subscribe(_ =>
                {
                    _animator.ResetTrigger("ContinuousAttack");

                    if (_actionView.HasStateBy(ATTACK))
                        _actionView.State.Value = _waitState;
                });

            _animTrigger.OnStateExitAsObservable()
                .Where(s => s.StateInfo.IsName("Down"))
                .Subscribe(_ =>
                {
                    _actionView.State.Value = _waitState;
                })
                .AddTo(this);
        }

        /// <summary>
        /// 攻撃状態に切り替えます
        /// </summary>
        void ChangeAttack()
        {
            _playerWeapon.Use();

            //連続攻撃
            if (_actionView.HasStateBy(ATTACK))
            {
                _animator.SetTrigger("ContinuousAttack");
            }
            _actionView.State.Value = _attackState;

        }

        /// <summary>
        /// 操作可能な状態か
        /// </summary>
        bool IsControllableState()
        {
            return (_actionView.HasStateBy(RUN)
                || _actionView.HasStateBy(WAIT)
                || _actionView.HasStateBy(ATTACK));
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
        void CheckTrigger(Collider collider)
        {
            GetItem(collider);
            ReceiveDamageBy(collider);
        }

        /// <summary>
        /// アイテムを獲得します
        /// </summary>
        /// <param name="collider"></param>
        void GetItem(Collider collider)
        {
            if (collider.TryGetComponent(out IGetableItem item) == false) return;

            GetPointBy(collider);
            GetHealItemBy(collider);
            GetSpWeaponItemBy(collider);

            _scoreModel.AddScore(item.Score);
            item.Destroy();
        }

        /// <summary>
        /// 衝突時に確認します
        /// </summary>
        void CheckCollision(UnityEngine.Collision collision)
        {
            ReceiveDamageBy(collision.collider);
        }

        /// <summary>
        /// 回復アイテムの取得を試みます
        /// </summary>
        /// <param name="collider"></param>
        void GetHealItemBy(Collider collider)
        {
            if (collider.TryGetComponent(out IHealable healItem))
            {
                AddHp(healItem.HealingPower);
            }
        }

        /// <summary>
        /// ポイントの取得を試みます
        /// </summary>
        void GetPointBy(Collider collider)
        {
            if (collider.TryGetComponent(out IPoint pointItem))
            {
                _soundManager.PlaySE(POINT_GET);
                _pointModel.AddPoint(pointItem.Point);
            }
        }

        /// <summary>
        /// Sp武器の取得を試みます
        /// </summary>
        /// <param name="collider"></param>
        void GetSpWeaponItemBy(Collider collider)
        {
            //Sp武器ならスコアを獲得し、自身のアイテム欄にセット
            if (collider.TryGetComponent(out ISpWeaponItem spWeaponItem) == false) return;

            SpWeaponData spWeaponData
                = _spWeaponDataList.FindSpWeaponDataByType(spWeaponItem.Type);

            if (spWeaponData == null) return;
            
            //武器が違う場合のみセットする
            if (_currentSpWeapon?.Type != spWeaponData.Type)
            {
                _spWeaponView.SetIcon(spWeaponData.UIIcon);

                ISpPlayerWeaponInvoker invoker =
                    container.InstantiatePrefab(spWeaponData.SpWeaponInvoker)
                    .GetComponent<ISpPlayerWeaponInvoker>();

                _currentSpWeapon = invoker;
                _currentSpWeapon.SetPlayerTransform(transform);
                _currentSpWeapon.SetPower(spWeaponData.Power);
            }

            //SE
            _soundManager.PlaySE(SP_WEAPON_ITEM_GET);
        }

        /// <summary>
        /// ダメージを受けるか確認します
        /// </summary>
        void ReceiveDamageBy(Collider collider)
        {
            if (_isBlink) return;
            if (_actionView.HasStateBy(ATTACK)) return;//攻撃中はダメージを受けない
            if (collider.TryGetComponent(out IPlayerAttacker attacker))
            {
                _soundManager.PlaySE(DAMAGED);
                _hpModel.ReduceHp(attacker.Power);
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
            _hpModel.AddHp(hp);
            _soundManager.PlaySE(HP_UP);
        }

        /// <summary>
        /// 入力の有無でプレイヤーの状態を切り替えます
        /// </summary>
        /// <param name="input"></param>
        void ChangeStateByInput(Vector2 input)
        {
            if (input.magnitude != 0)
                _actionView.State.Value = _runState;
            else
                _actionView.State.Value = _waitState;
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
            _actionView.State.Value = _downState;
            PlayerBlinks().Forget();//点滅処理
        }

        public void ChangeDead()
        {
            _actionView.State.Value = _deadState;
            _directionModel.SetIsGameOver(true);
        }

        public void ChangeJoy()
        {
            _actionView.State.Value = _joyState;
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