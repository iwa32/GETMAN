using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using StateView;
using static StateType;

namespace EnemyPresenter
{
    public class EnemyPresenter : MonoBehaviour
    {
        [SerializeField]
        [Header("EnemyDataList(ScriptableObjectを設定)")]
        EnemyDataList _enemyDataList;

        [SerializeField]
        [Header("エネミーのアクション用スクリプトを設定")]
        ActionView _actionView;

        [SerializeField]
        [Header("待機状態のスクリプトを設定")]
        WaitView _waitView;

        [SerializeField]
        [Header("移動状態のスクリプトを設定")]
        RunView _runView;

        [SerializeField]
        [Header("ダウン状態のスクリプトを設定")]
        DownView _downView;

        [SerializeField]
        [Header("デッド状態のスクリプトを設定")]
        DeadView _deadView;

        [SerializeField]
        [Header("攻撃状態のスクリプトを設定")]
        AttackView _attackView;

        EnemyData _enemyData;
        Rigidbody _rigidbody;
        Animator _animator;

        // Start is called before the first frame update
        void Awake()
        {
            //todo 後で清書
            _enemyData = _enemyDataList.GetEnemyDataList[0];
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
