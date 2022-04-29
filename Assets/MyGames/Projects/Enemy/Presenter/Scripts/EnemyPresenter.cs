using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Zenject;
using Trigger;
using EnemyModel;
using GameModel;
using Cysharp.Threading.Tasks;
using Collision;
using GlobalInterface;
using EnemyDataList;
using EnemyStates;
using EnemyActions;

namespace EnemyPresenter
{
    /// <summary>
    /// エネミー共通のpresenterクラス
    /// </summary>
    public abstract class EnemyPresenter : MonoBehaviour
    {
        #region//インスペクターから設定
        [SerializeField]
        [Header("HpBarを設定する")]
        HpBar.HpBar _hpBar;

        [SerializeField]
        [Header("エネミーの種類を設定")]
        protected EnemyType _type;
        #endregion

        #region//フィールド
        //---接触・衝突---
        ObservableTrigger _trigger;
        ObservableCollision _collision;
        //状態と実処理
        protected EnemyCommonStates _enemyCommonStates;
        protected EnemyCommonActions _enemyCommonActions;
        //---モデル---
        protected IHpModel _hpModel;
        protected IDirectionModel _directionModel;
        #endregion

        #region//プロパティ
        public IReadOnlyReactiveProperty<bool> IsDead => _enemyCommonStates.IsDead;
        public EnemyType Type => _type;
        #endregion

        [Inject]
        public void Construct(
            IHpModel hp,
            IDirectionModel direction
        )
        {
            _hpModel = hp;
            _directionModel = direction;
        }

        // Start is called before the first frame update
        protected void ManualAwake()
        {
            //接触、衝突
            _trigger = GetComponent<ObservableTrigger>();
            _collision = GetComponent<ObservableCollision>();
            _enemyCommonStates = GetComponent<EnemyCommonStates>();
            _enemyCommonActions = GetComponent<EnemyCommonActions>();
            _enemyCommonStates.ManualAwake();
            _enemyCommonActions.ManualAwake();
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        public virtual void Initialize(EnemyData data)
        {
            _hpBar.SetMaxHp(data.Hp);
            InitializeModel(data);
            _enemyCommonStates.Initialize();
            _enemyCommonActions.Initialize(data);

            Bind();
        }

        /// <summary>
        /// モデルの初期化を行います
        /// </summary>
        void InitializeModel(EnemyData data)
        {
            _hpModel.SetHp(data.Hp);
        }

        void Bind()
        {
            //model to view
            //HPBarへの設定
            _hpModel.Hp
                .TakeUntil(_enemyCommonStates.IsDead.Where(isDead => isDead))
                .Subscribe(hp => _hpBar.SetHp(hp))
                .AddTo(this);

            //trigger, collisionの取得
            _trigger.OnTriggerEnter()
                .TakeUntil(_enemyCommonStates.IsDead.Where(isDead => isDead))
                .Where(_ => _directionModel.CanGame())
                .Where(_ => _enemyCommonStates.HasStateBy(StateType.DEAD) == false)
                .Subscribe(collider => CheckCollider(collider))
                .AddTo(this);
        }

        //presenter---
        #region //abstractMethod
        /// <summary>
        /// 接触したコライダーを確認します
        /// </summary>
        /// <param name="collider"></param>
        public abstract void CheckCollider(Collider collider);

        /// <summary>
        /// 衝突を確認します
        /// </summary>
        public abstract void CheckCollision(UnityEngine.Collision collision);
        #endregion

        /// <summary>
        /// プレイヤーの武器に接触したか
        /// </summary>
        protected void CheckPlayerWeaponBy(Collider collider)
        {
            if (collider.TryGetComponent(out IEnemyAttacker attacker))
            {
                if (_enemyCommonStates.IsDown) return;
                //hpを減らす
                _hpModel.ReduceHp(attacker.Power);
                _enemyCommonStates.ChangeStateByDamege(_hpModel.Hp.Value);
            }
        }

        /// <summary>
        /// ステージに配置します
        /// </summary>
        /// <param name="stageView"></param>
        public void PlaceOnStage(StageView.StageView stageView)
        {
            _enemyCommonActions.SetStageInformation(stageView, _type);
            _enemyCommonStates.DefaultState();
        }
    }
}
