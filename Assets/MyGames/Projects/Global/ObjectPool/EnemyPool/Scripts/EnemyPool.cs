using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EP = EnemyPresenter;
using OP = ObjectPool;


namespace EnemyPool
{
    public class EnemyPool : OP.ObjectPool,
        IEnemyPool
    {
        Dictionary<EnemyType, List<EP.EnemyPresenter>> _enemyPoolList
            = new Dictionary<EnemyType, List<EP.EnemyPresenter>>();

        /// <summary>
        /// オブジェクトプールを作成する
        /// </summary>
        /// <param name="enemyData"></param>
        /// <param name="maxEnemyCount"></param>
        public void CreatePool(EP.EnemyPresenter enemyPrefab, int maxEnemyCount)
        {
            //対象のエネミーのプールを作成します
            _enemyPoolList[enemyPrefab.Type] = new List<EP.EnemyPresenter>();

            for (int i = 0; i < maxEnemyCount; i++)
            {
                EP.EnemyPresenter enemy
                    = Create(enemyPrefab);

                _enemyPoolList[enemy.Type].Add(enemy);
                enemy.gameObject?.SetActive(false);
            }
        }

        /// <summary>
        /// プールを取得します
        /// </summary>
        /// <returns></returns>
        public EP.EnemyPresenter GetPool(EnemyType type)
        {
            return GetBehaviourByList(_enemyPoolList[type])
                ?.GetComponent<EP.EnemyPresenter>();
        }

        /// <summary>
        /// エネミーの作成
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        EP.EnemyPresenter Create(EP.EnemyPresenter prefab)
        {
            EP.EnemyPresenter enemy
                = container.InstantiatePrefab(prefab)
                .GetComponent<EP.EnemyPresenter>();

            return enemy;
        }
    }
}