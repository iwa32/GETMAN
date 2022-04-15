using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UniRx;
using UniRx.Triggers;
using Zenject;
using StateView;
using Trigger;
using EnemyView;
using EnemyModel;
using GameModel;
using StrategyView;
using Cysharp.Threading.Tasks;
using Collision;
using GlobalInterface;
using EnemyDataList;
using StageObject;
using StageView;

namespace EnemyPresenter
{
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
        //---状態---
        protected ActionView _actionView;//エネミーのアクション用スクリプト
        protected WaitState _waitState;//待機
        protected RunState _runState;//走行
        DownState _downState;//ダウン
        DeadState _deadState;//デッド
        //---接触・衝突---
        ObservableTrigger _trigger;
        ObservableCollision _collision;
        //---アニメーション---
        Animator _animator;
        protected ObservableStateMachineTrigger _animTrigger;//stateMachineの監視
        Collider _collider;
        protected NavMeshAgent _navMeshAgent;
        //---フラグ---
        bool _isDown;
        protected BoolReactiveProperty _isDead = new BoolReactiveProperty();
        bool _isInitialized;
        //---モデル---
        protected IHpModel _hpModel;
        EnemyModel.IScoreModel _enemyScoreModel;//enemyの保持するスコア
        GameModel.IScoreModel _gameScoreModel;//gameの保持するスコア
        protected IPowerModel _powerModel;
        IDirectionModel _directionModel;
        EnemyData _enemyData;
        GetableItem _dropItemPool;//生成済みのドロップアイテムの保管場所
        #endregion

        #region//プロパティ
        public IReadOnlyReactiveProperty<bool> IsDead => _isDead;
        public EnemyType Type => _type;
        #endregion

        // Start is called before the first frame update
        protected void Awake()
        {
            //共通のstate
            _actionView = GetComponent<ActionView>();
            _waitState = GetComponent<WaitState>();
            _runState = GetComponent<RunState>();
            _downState = GetComponent<DownState>();
            _deadState = GetComponent<DeadState>();
            //接触、衝突
            _trigger = GetComponent<ObservableTrigger>();
            _collision = GetComponent<ObservableCollision>();
            _collider = GetComponent<Collider>();
            //アニメーション
            _animator = GetComponent<Animator>();
            _animTrigger = _animator.GetBehaviour<ObservableStateMachineTrigger>();
            //ナビメッシュ
            _navMeshAgent = GetComponent<NavMeshAgent>();
        }

        [Inject]
        public void Construct(
            IHpModel hp,
            IPowerModel power,
            EnemyModel.IScoreModel enemyScore,
            GameModel.IScoreModel gameScore,
            IDirectionModel direction
        )
        {
            _hpModel = hp;
            _powerModel = power;
            _enemyScoreModel = enemyScore;
            _gameScoreModel = gameScore;
            _directionModel = direction;
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        public void Initialize(EnemyData data)
        {
            //awakeでanimeTriggerを取得した場合アニメーションの終了検知がうまくいかない場合があるため、こちらで設定する
            _animTrigger = _animator.GetBehaviour<ObservableStateMachineTrigger>();

            _enemyData = data;
            _hpBar.SetMaxHp(data.Hp);
            InitializeModel(data.Hp, data.Power, data.Score);

            _collider.enabled = true;
            _navMeshAgent.isStopped = false;
            _navMeshAgent.speed = data.Speed;
            _isDead.Value = false;
            _isInitialized = true;
            Bind();
        }

        /// <summary>
        /// モデルの初期化を行います
        /// </summary>
        void InitializeModel(int hp, int power, int score)
        {
            _hpModel.SetHp(hp);
            _powerModel.SetPower(power);
            _enemyScoreModel.SetScore(score);
        }

        void Bind()
        {
            //model to view
            //HPBarへの設定
            _hpModel.Hp
                .Subscribe(hp => _hpBar.SetHp(hp))
                .AddTo(this);

            //trigger, collisionの取得
            _trigger.OnTriggerEnter()
                .Where(_ => _directionModel.CanGame()
                && (_actionView.HasStateBy(StateType.DEAD) == false)
                )
                .Subscribe(collider => CheckCollider(collider))
                .AddTo(this);

            //view to model
            //状態の監視
            _actionView.State
                .Where(x => x != null)
                .Subscribe(x => _actionView.ChangeState(x.State))
                .AddTo(this);

            //アニメーションの監視
            //down
            _animTrigger.OnStateExitAsObservable()
                .Where(s => s.StateInfo.IsName("Down"))
                .Subscribe(_ => DefaultState())
                .AddTo(this);

            //dead
            _animTrigger.OnStateExitAsObservable()
                .Where(s => s.StateInfo.IsName("Dead"))
                .Subscribe(_ =>
                {
                    gameObject.SetActive(false);
                    _isDead.Value = false;
                }).AddTo(this);

            //死亡しているのに生存している場合、2秒後に破棄します
            _isDead
                .Where(_isDead => _isDead == true)
                .Delay(TimeSpan.FromSeconds(2))
                .Subscribe(_ =>
                {
                    _isDead.Value = false;

                    if (_isInitialized) return;
                    gameObject.SetActive(false);
                })
                .AddTo(this);


            //FixedUpdate
            this.FixedUpdateAsObservable()
                .Subscribe(_ =>
                {
                    _actionView.Action();
                })
                .AddTo(this);
        }

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

        /// <summary>
        /// 初期時、通常時の状態を設定します
        /// </summary>
        public abstract void DefaultState();

        /// <summary>
        /// ステージ情報を設定します
        /// </summary>
        /// <param name="stageView"></param>
        public abstract void SetStageInformation(StageView.StageView stageView);
        #endregion

        /// <summary>
        /// プレイヤーの武器に接触したか
        /// </summary>
        protected void CheckPlayerWeaponBy(Collider collider)
        {
            if (collider.TryGetComponent(out IEnemyAttacker attacker))
            {
                if (_isDown) return;
                //hpを減らす
                _hpModel.ReduceHp(attacker.Power);
                ChangeStateByDamege();
            }
        }

        /// <summary>
        /// 配置を行います
        /// </summary>
        /// <param name="transform"></param>
        protected void SetTransform(Transform targetTransform)
        {
            //navMeshAgentのオブジェクトをtransform.positionに代入するとうまくいかないためwarpを使用
            _navMeshAgent.Warp(targetTransform.position);
            transform.rotation = targetTransform.rotation;
        }

        /// <summary>
        /// ダメージによって状態を切り替えます
        /// </summary>
        protected void ChangeStateByDamege()
        {
            if (_hpModel.Hp.Value > 0)
                ChangeDown();
            else
                ChangeDead();
        }

        void ChangeDown()
        {
            _isDown = true;
            _actionView.State.Value = _downState;

            ResetDown().Forget();
        }

        async UniTask ResetDown()
        {
            await UniTask.Yield();
            _isDown = false;
        }

        void ChangeDead()
        {
            _collider.enabled = false;//スコア二重取得防止
            _navMeshAgent.isStopped = true;
            _actionView.State.Value = _deadState;
            _gameScoreModel.AddScore(_enemyScoreModel.Score.Value);
            _isDead.Value = true;
            JudgeDrop();
            _isInitialized = false;
        }

        void JudgeDrop()
        {
            //抽選 todo 後にクラスにまとめる

            bool isDrop = false;
            //1~100までの数を取得
            int randomValue = UnityEngine.Random.Range(1, 101);
            //抽選
            if (_enemyData.ItemDropRate >= randomValue)
            {
                isDrop = true;
            }

            bool canDrop = (isDrop && _enemyData.DropItem != null);
            if (canDrop == false) return;

            //ドロップする
            //プールにない場合、生成する
            if (_dropItemPool == null)
            {
                GetableItem dropItem
                = Instantiate(
                    _enemyData.DropItem,
                    transform.position,
                    _enemyData.DropItem.transform.rotation
                    );

                _dropItemPool = dropItem;
                return;
            }

            //ある場合位置を更新して表示
            _dropItemPool.transform.position = transform.position;
            _dropItemPool.gameObject?.SetActive(true);
        }
    }
}
