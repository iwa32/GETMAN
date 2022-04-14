using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using EP = EnemyPresenter;
using EnemyDataList;
using ObjectPool;

namespace BehaviourFactory
{
    public class EnemyFactory : BehaviourFactory<EP.EnemyPresenter>
    {
        [SerializeField]
        [Header("EnemyDataListのスクリプタブルオブジェクトを設定")]
        EnemyDataList.EnemyDataList _enemyDataList;

        IEnemyPool _enemyPool;

        [Inject]
        public void Construct(IEnemyPool enemyPool)
        {
            _enemyPool = enemyPool;
        }

        /// <summary>
        /// エネミーのデータを設定します
        /// </summary>
        /// <param name="enemyTypes"></param>
        public void SetEnemyData(EP.EnemyPresenter[] enemies, int maxEnemyCount)
        {
            _enemyPool.CreatePool(enemies, maxEnemyCount);
        }

        /// <summary>
        /// エネミーを生成します
        /// </summary>
        public override EP.EnemyPresenter Create()
        {
            //poolから取得します
            EP.EnemyPresenter enemy = _enemyPool.GetPool();

            if (enemy != null)
            {
                //enemyのデータを取得します
                EnemyData data =
                    _enemyDataList.GetEnemyDataList
                    .First(data => data.EnemyType == enemy.Type);

                enemy.Initialize(data);
            }

            return enemy;
        }
    }

}