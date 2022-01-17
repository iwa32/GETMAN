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

namespace EnemyPresenter
{
    public class EnemyPresenter : MonoBehaviour, IEnemy, IDamager
    {
        #region//インスペクターから設定
        [SerializeField]
        [Header("EnemyDataList(ScriptableObjectを設定)")]
        EnemyDataList _enemyDataList;

        [SerializeField]
        [Header("追跡エリアのコンポーネントを設定")]
        TrackingAreaView _trackingAreaView;
        #endregion

        #region//フィールド
        //状態
        ActionView _actionView;//エネミーのアクション用スクリプト
        WaitView _waitView;//待機状態のスクリプト
        RunView _runView;//移動状態のスクリプト
        DownView _downView;//ダウン状態のスクリプト
        DeadView _deadView;//デッド状態のスクリプト
        AttackView _attackView;//攻撃状態のスクリプト
        TrackView _trackView;//追跡状態のスクリプト
        TriggerView.TriggerView _triggerView;//接触判定スクリプト
        CollisionView _collisionView;//衝突判定スクリプト
        EnemyData _enemyData;
        Animator _animator;
        Collider _collider;
        ObservableStateMachineTrigger _animTrigger;//アニメーションの監視
        NavMeshAgent _navMeshAgent;
        //モデル
        IHpModel _hpModel;
        EnemyModel.IScoreModel _enemyScoreModel;//enemyの保持するスコア
        GameModel.IScoreModel _gameScoreModel;//gameの保持するスコア
        IPowerModel _powerModel;
        #endregion

        #region//プロパティ
        public int Damage => _powerModel.Power.Value;
        #endregion

        // Start is called before the first frame update
        void Awake()
        {
            //todo 後で清書
            _enemyData = _enemyDataList.GetEnemyDataList[0];

            _actionView = GetComponent<ActionView>();
            _waitView = GetComponent<WaitView>();
            _runView = GetComponent<RunView>();
            _downView = GetComponent<DownView>();
            _deadView = GetComponent<DeadView>();
            _attackView = GetComponent<AttackView>();
            _trackView = GetComponent<TrackView>();
            _triggerView = GetComponent<TriggerView.TriggerView>();
            _collisionView = GetComponent<CollisionView>();
            _collider = GetComponent<Collider>();
            _animator = GetComponent<Animator>();
            _animTrigger = _animator.GetBehaviour<ObservableStateMachineTrigger>();
            _navMeshAgent = GetComponent<NavMeshAgent>();
        }

        [Inject]
        public void Construct(
            IHpModel hp,
            IPowerModel power,
            EnemyModel.IScoreModel enemyScore,
            GameModel.IScoreModel gameScore
        )
        {
            _hpModel = hp;
            _powerModel = power;
            _enemyScoreModel = enemyScore;
            _gameScoreModel = gameScore;
        }

        void Start()
        {
            Initialize();
            Bind();
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        void Initialize()
        {
            InitializeModel();
            _runView.DelAction = Run;
            _trackView.DelAction = Track;
            _actionView.State.Value = _runView;
            _navMeshAgent.speed = _enemyData.Speed;
        }

        /// <summary>
        /// モデルの初期化を行います
        /// </summary>
        void InitializeModel()
        {
            _hpModel.SetHp(_enemyData.Hp);
            _powerModel.SetPower(_enemyData.Power);
            _enemyScoreModel.SetScore(_enemyData.Score);
        }

        void Bind()
        {
            //model to view
            _hpModel.Hp.Subscribe(hp => Debug.Log(hp)).AddTo(this);

            //trigger, collisionの取得
            _triggerView.OnTrigger()
                .ThrottleFirst(TimeSpan.FromMilliseconds(1000))
                .Subscribe(collider => CheckCollider(collider))
                .AddTo(this);

            _collisionView.OnCollision()
                .ThrottleFirst(TimeSpan.FromMilliseconds(1000))
                .Subscribe(collision => CheckCollision(collision))
                .AddTo(this);

            //プレイヤーの追跡
            _trackingAreaView.CanTrack
                .Subscribe(canTrack => CheckTracking(canTrack))
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
                .Where(s => s.StateInfo.normalizedTime >= 1)
                .Subscribe(_ =>
                {
                    _actionView.State.Value = _runView;
                }).AddTo(this);
            //dead
            _animTrigger.OnStateUpdateAsObservable()
                .Where(s => s.StateInfo.IsName("Dead"))
                .Where(s => s.StateInfo.normalizedTime >= 1)
                .Subscribe(_ =>
                {
                    Destroy(gameObject);
                }).AddTo(this);
        }

        //todo 後に修正
        void FixedUpdate()
        {
            _actionView.Action();
            //進行方向を変える
            //if (checkCollision.isOn)
            //{
            //    //向きを回転
            //    RandomRotateEnemy();
            //}
        }

        /// <summary>
        /// 接触したコライダーを確認します
        /// </summary>
        /// <param name="collider"></param>
        void CheckCollider(Collider collider)
        {
            //武器に接触でダメージを受ける
            TryCheckPlayerWeapon(collider);
        }

        /// <summary>
        /// 衝突を確認します
        /// </summary>
        void CheckCollision(Collision collision)
        {
            //壁に接触で向きを変える
        }

        /// <summary>
        /// プレイヤーの武器に接触したか
        /// </summary>
        void TryCheckPlayerWeapon(Collider collider)
        {
            if (collider.TryGetComponent(out IPlayerWeapon playerWeapon))
            {
                //hpを減らす
                _hpModel.ReduceHp(playerWeapon.Power);
                ChangeStateByDamege();
            }
        }

        /// <summary>
        /// ダメージによって状態を切り替えます
        /// </summary>
        void ChangeStateByDamege()
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
            _actionView.State.Value = _deadView;
            _gameScoreModel.AddScore(_enemyData.Score);
            _collider.enabled = false;//スコア二重取得防止
        }

        /// <summary>
        /// 追跡の確認をします
        /// </summary>
        /// <param name="canTrack"></param>
        void CheckTracking(bool canTrack)
        {
            //追跡もしくは前方を走ります
            if (canTrack)
                _actionView.State.Value = _trackView;
            else
                _actionView.State.Value = _runView;

        }

        /// <summary>
        /// 走ります
        /// </summary>
        void Run()
        {
            //前方を移動します
            _navMeshAgent.SetDestination(transform.position + transform.forward);
        }

        /// <summary>
        /// 追跡します
        /// </summary>
        void Track()
        {
            _navMeshAgent.SetDestination(_trackingAreaView.TargetPlayerPosition);
        }

        /// <summary>
        /// 進行方向を変える
        /// </summary>
        void RandomRotateEnemy()
        {
            //進行方向はランダム
            int dice = RandomDice(1, 5);
            int dirAngle = 90;

            dirAngle *= dice;

            //すでに同じ方向を向いてたら処理を行わない
            if (transform.localEulerAngles.x == dirAngle) return;
            //オイラー値をQuaternionに変換する。引数はz, x, y
            transform.rotation = Quaternion.Euler(0, dirAngle, 0);
        }

        /// <summary>
        /// ランダムな数値を算出
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns>ランダムな数値を出力</returns>
        int RandomDice(int min, int max)
        {
            return UnityEngine.Random.Range(min, max);
        }

    }
}
