using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using StateView;
using TriggerView;
using static StateType;

namespace EnemyPresenter
{
    public class EnemyPresenter : MonoBehaviour
    {
        [SerializeField]
        [Header("EnemyDataList(ScriptableObjectを設定)")]
        EnemyDataList _enemyDataList;

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
            _enemyData.Hp.Subscribe(_ => Debug.Log(""));

            //trigger, collisionの取得
            _triggerView.OnTrigger()
                .Subscribe(_ => Debug.Log("trigger"));

            _collisionView.OnCollision()
                .Subscribe(_ => Debug.Log("Collision"));


            //view to model
            //状態の監視
            _actionView.State
                .Where(x => x != null)
                .Subscribe(x =>
                {
                    _actionView.ChangeState(x.State);
                });

            //wait
            //run
            //attack
            //tracking
            //down
            //dead
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
