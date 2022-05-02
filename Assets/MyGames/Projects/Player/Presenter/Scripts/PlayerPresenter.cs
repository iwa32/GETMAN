using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using Zenject;
using PlayerModel;
using GameModel;
using PlayerView;
using Trigger;
using Collision;
using StageObject;
using SoundManager;
using static StateType;
using static SEType;
using GlobalInterface;
using PlayerActions;
using PlayerStates;

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
        [Header("HPのUIを設定")]
        HpView _hpView;

        [SerializeField]
        [Header("SP武器表示用のUIを設定")]
        SpWeaponView _spWeaponView;
        #endregion

        #region//フィールド
        ObservableTrigger _trigger;//接触判定スクリプト
        ObservableCollision _collision;//衝突判定スクリプト
        InputView _inputView;//プレイヤーの入力取得スクリプト
        PlayerActions.PlayerActions _playerActions;//プレイヤーの実行処理スクリプト
        PlayerStates.PlayerStates _playerStates;//プレイヤーの状態管理スクリプト
        IDirectionModel _directionModel;
        IWeaponModel _weaponModel;
        IHpModel _hpModel;
        IScoreModel _scoreModel;
        IPointModel _pointModel;
        ISoundManager _soundManager;
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
        public void Awake()
        {
            _trigger = GetComponent<ObservableTrigger>();
            _collision = GetComponent<ObservableCollision>();
            _playerStates = GetComponent<PlayerStates.PlayerStates>();
            _playerActions = GetComponent<PlayerActions.PlayerActions>();
            _inputView = GetComponent<InputView>();
            _playerStates.ManualAwake();
            _playerActions.ManualAwake();
        }

        /// <summary>
        /// プレイヤーのUIを設定します
        /// </summary>
        /// <param name="hpView"></param>
        /// <param name="spWeaponView"></param>
        public void SetPlayerUI(HpView hpView, SpWeaponView spWeaponView)
        {
            _hpView = hpView;
            _spWeaponView = spWeaponView;
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        public void Start()
        {
            InitializeModel();
            _playerStates.Initialize();
            _playerActions.Initialize();
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
        /// リセットします
        /// </summary>
        public void ResetData()
        {
            _playerStates.DefaultState();
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

            //入力の監視
            _inputView.InputDirection
                .Where(_ => _directionModel.CanGame())
                .Subscribe(input => _playerStates.ChangeStateByInput(input))
                .AddTo(this);

            //攻撃入力
            //剣での攻撃
            _inputView.IsFired
                .Where(x => (x == true)
                && _directionModel.CanGame()
                && _playerStates.IsControllableState())
                .Subscribe(_ => _playerStates.ChangeAttack())
                .AddTo(this);

            //SP武器での攻撃
            _inputView.IsSpAttack
                .Where(x => (x == true)
                && _directionModel.CanGame()
                && _playerStates.IsControllableState()
                )
                .Subscribe(_ => _playerActions.DoSpAttack())
                .AddTo(this);
        }

        /// <summary>
        /// fixedUpdate処理
        /// </summary>
        public void ManualFixedUpdate()
        {
            _playerStates.ManualFixedUpdate();
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

            _playerActions.SetSpWeapon(spWeaponItem.Type);

            if (_playerActions.SpWeaponData == null) return;

            _spWeaponView.SetIcon(_playerActions.SpWeaponData.UIIcon);
            _soundManager.PlaySE(SP_WEAPON_ITEM_GET);
        }

        /// <summary>
        /// ダメージを受けるか確認します
        /// </summary>
        void ReceiveDamageBy(Collider collider)
        {
            if (_playerActions.IsBlink) return;
            if (collider.TryGetComponent(out IPlayerAttacker attacker))
            {
                _soundManager.PlaySE(DAMAGED);
                _hpModel.ReduceHp(attacker.Power);
                _playerStates.ChangeStateByDamage(_hpModel.Hp.Value);
                _playerActions.KnockBack(collider?.gameObject);
            }
        }

        public void PlaceOnStage(Transform startingTransform)
        {
            _playerActions.SetTransform(startingTransform);
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
    }
}