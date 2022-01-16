using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Zenject;
using StateView;
using TriggerView;
using EnemyModel;
using static StateType;

namespace EnemyPresenter
{
    public class EnemyPresenter : MonoBehaviour
    {
        #region//インスペクターから設定
        [SerializeField]
        [Header("EnemyDataList(ScriptableObjectを設定)")]
        EnemyDataList _enemyDataList;
        #endregion

        #region//フィールド
        ActionView _actionView;//エネミーのアクション用スクリプト
        WaitView _waitView;//待機状態のスクリプト
        RunView _runView;//移動状態のスクリプト
        DownView _downView;//ダウン状態のスクリプト
        DeadView _deadView;//デッド状態のスクリプト
        AttackView _attackView;//攻撃状態のスクリプト
        TriggerView.TriggerView _triggerView;//接触判定スクリプト
        CollisionView _collisionView;//衝突判定スクリプト
        EnemyData _enemyData;
        Rigidbody _rigidbody;
        Animator _animator;
        ObservableStateMachineTrigger _animTrigger;//アニメーションの監視
        IHpModel _hpModel;
        IScoreModel _scoreModel;
        IPowerModel _powerModel;
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
            _triggerView = GetComponent<TriggerView.TriggerView>();
            _collisionView = GetComponent<CollisionView>();
            _rigidbody = GetComponent<Rigidbody>();
            _animator = GetComponent<Animator>();
            _animTrigger = _animator?.GetBehaviour<ObservableStateMachineTrigger>();
        }

        [Inject]
        public void Construct(
            IHpModel hp,
            IScoreModel score,
            IPowerModel power
        )
        {
            _hpModel = hp;
            _scoreModel = score;
            _powerModel = power;
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
            _runView.DelAction = Run;
            _actionView.State.Value = _runView;
        }

        void Bind()
        {
            //model to view
            _enemyData.Hp.Subscribe(hp => Debug.Log(hp)).AddTo(this);

            //trigger, collisionの取得
            _triggerView.OnTrigger()
                .Subscribe(collider => CheckCollider(collider)).AddTo(this);

            _collisionView.OnCollision()
                .Subscribe(collision => CheckCollision(collision)).AddTo(this);


            //view to model
            //状態の監視
            _actionView.State
                .Where(x => x != null)
                .Subscribe(x =>
                {
                    _actionView.ChangeState(x.State);
                }).AddTo(this);

            //wait
            //run
            //attack
            //tracking
            //down
            //dead

            //アニメーションの監視
            //todonull修正
            //_animTrigger.OnStateUpdateAsObservable()
            //    .Where(s => s.StateInfo.IsName("Down"))
            //    .Where(s => s.StateInfo.normalizedTime >= 1)
            //    .Subscribe(_ =>
            //    {
            //        _actionView.State.Value = _waitView;
            //    }).AddTo(this);
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

            //SetAnimation();
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
            //壁に接触で武器を変える
        }

        /// <summary>
        /// プレイヤーの武器に接触したか
        /// </summary>
        void TryCheckPlayerWeapon(Collider collider)
        {
            if (collider.TryGetComponent(out IPlayerWeapon playerWeapon))
            {
                //hpを減らすtodo 後でmodelに変更
                _enemyData.ReduceHp(playerWeapon.Power);
                ChangeStateByDamege();

            }
        }

        /// <summary>
        /// ダメージによって状態を切り替えます
        /// </summary>
        void ChangeStateByDamege()
        {
            if (_enemyData.Hp.Value > 0)
            {
                _actionView.State.Value = _downView;
                //一定時間無敵
            }
            else
            {
                _actionView.State.Value = _deadView;
                //スコア加算
                //加算したらオブジェクトの削除
            }
        }

        /// <summary>
        /// 走ります
        /// </summary>
        void Run()
        {
            WalkToForward();
        }

        /// <summary>
        /// 前方に歩く
        /// </summary>
        void WalkToForward()
        {
            //velocityによる移動
            _rigidbody.velocity = transform.forward * _enemyData.Speed.Value;
            //isWalk = true;
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
            return Random.Range(min, max);
        }

    }
}
