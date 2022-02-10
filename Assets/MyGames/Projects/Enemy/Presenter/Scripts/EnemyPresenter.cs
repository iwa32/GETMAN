using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UniRx;
using UniRx.Triggers;
using Zenject;
using StateView;
using TriggerView;
using EnemyView;
using EnemyModel;
using GameModel;
using StageObject;

namespace EnemyPresenter
{
    public abstract class EnemyPresenter : MonoBehaviour
    {
        #region//インスペクターから設定
        [SerializeField]
        [Header("HpBarを設定する")]
        HpBar.HpBar _hpBar;
        #endregion

        #region//フィールド
        //状態
        protected ActionView _actionView;//エネミーのアクション用スクリプト
        DownView _downView;//ダウン状態のスクリプト
        DeadView _deadView;//デッド状態のスクリプト
        TriggerView.TriggerView _triggerView;//接触判定スクリプト
        CollisionView _collisionView;//衝突判定スクリプト
        Animator _animator;
        Collider _collider;
        protected ObservableStateMachineTrigger _animTrigger;//アニメーションの監視
        protected NavMeshAgent _navMeshAgent;
        //フラグ
        protected BoolReactiveProperty _isDead = new BoolReactiveProperty();//死亡フラグ
        //モデル
        protected IHpModel _hpModel;
        EnemyModel.IScoreModel _enemyScoreModel;//enemyの保持するスコア
        GameModel.IScoreModel _gameScoreModel;//gameの保持するスコア
        protected IPowerModel _powerModel;
        IDirectionModel _directionModel;
        #endregion

        #region//プロパティ
        public IReadOnlyReactiveProperty<bool> IsDead => _isDead;
        #endregion

        // Start is called before the first frame update
        protected void Awake()
        {
            //共通のstate
            _actionView = GetComponent<ActionView>();
            _downView = GetComponent<DownView>();
            _deadView = GetComponent<DeadView>();
            //接触、衝突
            _triggerView = GetComponent<TriggerView.TriggerView>();
            _collisionView = GetComponent<CollisionView>();
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
        public void Initialize(int hp, int power, int speed, int score)
        {
            InitializeModel(hp, power, score);
            _navMeshAgent.speed = speed;
            _hpBar.SetMaxHp(hp);
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
            _triggerView.OnTriggerEnter()
                .Where(_ => _directionModel.CanGame()
                && (_actionView.HasActionBy(StateType.DEAD) == false)
                )
                .Subscribe(collider => CheckCollider(collider))
                .AddTo(this);

            //view to model
            //状態の監視
            _actionView.State
                .Where(x => x != null)
                .Subscribe(x =>
                {
                    _actionView.ChangeState(x.State);
                }).AddTo(this);

            //アニメーションの監視
            //down
            _animTrigger.OnStateUpdateAsObservable()
                .Where(s => s.StateInfo.IsName("Down"))
                .Where(s => s.StateInfo.normalizedTime >= s.StateInfo.length)
                .Subscribe(_ =>
                {
                    //ダウン後に死亡したら何もしない
                    if (_actionView.HasActionBy(StateType.DEAD)) return;
                    DefaultState();
                }
                )
                .AddTo(this);

            //dead
            _animTrigger.OnStateUpdateAsObservable()
                .Where(s => s.StateInfo.IsName("Dead"))
                .Where(s => s.StateInfo.normalizedTime >= s.StateInfo.length)
                .Subscribe(_ =>
                {
                    _isDead.Value = true;
                    Destroy(gameObject);
                }).AddTo(this);

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
        public abstract void CheckCollision(Collision collision);

        /// <summary>
        /// 初期時、通常時の状態を設定します
        /// </summary>
        public abstract void DefaultState();
        #endregion

        /// <summary>
        /// 配置を行います
        /// </summary>
        /// <param name="transform"></param>
        public void SetTransform(Transform transform)
        {
            //navMeshAgentのオブジェクトをtransform.positionに代入するとうまくいかないためwarpを使用
            _navMeshAgent.Warp(transform.position);
            transform.rotation = transform.rotation;
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
            _actionView.State.Value = _downView;
        }

        void ChangeDead()
        {
            _collider.enabled = false;//スコア二重取得防止
            _actionView.State.Value = _deadView;
            _gameScoreModel.AddScore(_enemyScoreModel.Score.Value);
        }
    }
}
