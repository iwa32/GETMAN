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
    public class EnemyPresenter : MonoBehaviour, IDamager
    {
        #region//インスペクターから設定
        [SerializeField]
        [Header("追跡エリアのコンポーネントを設定")]
        TrackingAreaView _trackingAreaView;

        [SerializeField]
        [Header("障害物チェックのコンポーネントを設定する")]
        ForwardObstacleCheckView _forwardObstacleCheckView;
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
        Animator _animator;
        Collider _collider;
        ObservableStateMachineTrigger _animTrigger;//アニメーションの監視
        NavMeshAgent _navMeshAgent;
        //フラグ
        BoolReactiveProperty _isDead = new BoolReactiveProperty();//死亡フラグ
        //モデル
        IHpModel _hpModel;
        EnemyModel.IScoreModel _enemyScoreModel;//enemyの保持するスコア
        GameModel.IScoreModel _gameScoreModel;//gameの保持するスコア
        IPowerModel _powerModel;
        IDirectionModel _directionModel;
        #endregion

        #region//プロパティ
        public int Damage => _powerModel.Power.Value;
        public IReadOnlyReactiveProperty<bool> IsDead => _isDead;
        #endregion

        // Start is called before the first frame update
        void Awake()
        {
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
            _runView.DelAction = Run;
            _trackView.DelAction = Track;
            _actionView.State.Value = _runView;
            _navMeshAgent.speed = speed;
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
            //_hpModel.Hp.Subscribe(hp => Debug.Log(hp)).AddTo(this);

            //trigger, collisionの取得
            _triggerView.OnTriggerEnter()
                .Where(_ => _directionModel.CanGame()
                && (_actionView.HasActionBy(StateType.DEAD) == false)
                )
                .ThrottleFirst(TimeSpan.FromMilliseconds(1000))
                .Subscribe(collider => CheckCollider(collider))
                .AddTo(this);

            //前方の衝突を監視
            _forwardObstacleCheckView.IsOn
                .Where(isOn => isOn == true
                && (_actionView.HasActionBy(StateType.TRACK) == false)
                )
                .Subscribe(_ => ChangeDirectionForRandom())
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
                    //エネミーの削除はStagePresenterで行います
                    _isDead.Value = true;
                }).AddTo(this);
        }

        //todo 後に修正
        void FixedUpdate()
        {
            _actionView.Action();
        }

        /// <summary>
        /// 接触したコライダーを確認します
        /// </summary>
        /// <param name="collider"></param>
        void CheckCollider(Collider collider)
        {
            //武器に接触でダメージを受ける
            CheckPlayerWeaponBy(collider);
        }

        /// <summary>
        /// 衝突を確認します
        /// </summary>
        void CheckCollision(Collision collision)
        {
            
        }

        /// <summary>
        /// プレイヤーの武器に接触したか
        /// </summary>
        void CheckPlayerWeaponBy(Collider collider)
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
            _collider.enabled = false;//スコア二重取得防止
            _actionView.State.Value = _deadView;
            _gameScoreModel.AddScore(_enemyScoreModel.Score.Value);
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
        /// ランダムに進行方向を変える
        /// </summary>
        void ChangeDirectionForRandom()
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
