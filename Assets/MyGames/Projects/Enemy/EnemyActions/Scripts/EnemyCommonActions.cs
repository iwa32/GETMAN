using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using StageObject;
using EnemyDataList;
using Zenject;
using GlobalInterface;

namespace EnemyActions
{
    /// <summary>
    /// エネミー共通の実行処理クラス
    /// </summary>
    public abstract class EnemyCommonActions : MonoBehaviour, IPlayerAttacker
    {
        //ドロップアイテムの高さ
        readonly Vector3 dropItemPositionUp = new Vector3(0, 0.5f, 0);

        Collider _collider;
        GetableItem _dropItemPool;
        protected EnemyData _enemyData;
        GameModel.IScoreModel _gameScoreModel;//gameの保持するスコア
        protected NavMeshAgent _navMeshAgent;
        protected int _power;

        #region//プロパティ
        public int Power => _power;
        #endregion

        [Inject]
        public void Construct(
            GameModel.IScoreModel gameScoreModel
        )
        {
            _gameScoreModel = gameScoreModel;
        }

        /// <summary>
        /// プレハブのインスタンス直後の処理
        /// </summary>
        public void ManualAwake()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _collider = GetComponent<Collider>();
        }

        public void Initialize(EnemyData data)
        {
            _enemyData = data;
            _collider.enabled = true;
            SetPower(data.Power);
            _navMeshAgent.isStopped = false;
            _navMeshAgent.speed = data.Speed;
        }

        public void SetPower(int power)
        {
            _power = power;
        }

        #region //abstractMethod
        /// <summary>
        /// ステージ情報を設定します
        /// </summary>
        /// <param name="stageView"></param>
        public abstract void SetStageInformation(StageView.StageView stageView, EnemyType type);
        #endregion

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

        public void JudgeDrop()
        {
            //抽選 todo 後にクラスにまとめる

            bool isDrop = false;
            //1~100までの数を取得
            int randomValue = Random.Range(1, 101);
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
                //真上に生成
                GetableItem dropItem
                = Instantiate(
                    _enemyData.DropItem,
                    transform.position + dropItemPositionUp,
                    _enemyData.DropItem.transform.rotation
                    );

                _dropItemPool = dropItem;
                return;
            }

            //ある場合位置を更新して表示
            _dropItemPool.transform.position = transform.position + dropItemPositionUp;
            _dropItemPool.gameObject?.SetActive(true);
        }

        public void Dead()
        {
            _collider.enabled = false;//スコア二重取得防止
            _navMeshAgent.isStopped = true;
            _gameScoreModel.AddScore(_enemyData.Score);
            JudgeDrop();
        }
    }
}