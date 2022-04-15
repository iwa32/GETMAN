using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EP = EnemyPresenter;
using Zenject;

namespace ObjectPool
{
    struct EnemyPoolData
    {
        public EnemyType _type;
        public List<EP.EnemyPresenter> _pool;
    }

    public class EnemyPool : IEnemyPool
    {
        List<EnemyPoolData> _enemyPoolList = new List<EnemyPoolData>();

        [Inject]
        DiContainer container;//動的生成したデータにDIできるようにする

        /// <summary>
        /// オブジェクトプールを作成する
        /// </summary>
        /// <param name="enemyData"></param>
        /// <param name="maxEnemyCount"></param>
        public void CreatePool(EP.EnemyPresenter enemyPrefab, int maxEnemyCount)
        {
            //対象のエネミーのプールを作成します
            EnemyPoolData poolData =
                new EnemyPoolData
                {
                    _type = enemyPrefab.Type,
                    _pool = new List<EP.EnemyPresenter>()
                };

            for (int i = 0; i < maxEnemyCount; i++)
            {
                EP.EnemyPresenter enemy
                    = Create(enemyPrefab);

                poolData._pool.Add(enemy);
                enemy.gameObject?.SetActive(false);
            }

            _enemyPoolList.Add(poolData);
        }

        /// <summary>
        /// プールを取得します
        /// </summary>
        /// <returns></returns>
        public EP.EnemyPresenter GetPool(EnemyType type)
        {
            List<EP.EnemyPresenter> poolEnemyList
                = _enemyPoolList.Find(poolData => poolData._type == type)._pool;

            foreach (EP.EnemyPresenter enemy in poolEnemyList)
            {
                if (enemy.gameObject.activeSelf)
                {
                    continue;
                }

                enemy.gameObject?.SetActive(true);
                return enemy;
            }

            return null;
        }

        /// <summary>
        /// エネミーの作成
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        EP.EnemyPresenter Create(EP.EnemyPresenter prefab)
        {
            EP.EnemyPresenter enemy
                = container.InstantiatePrefab(
                            prefab,
                            Vector3.zero,
                            Quaternion.identity,
                            null
                )
                .GetComponent<EP.EnemyPresenter>();

            return enemy;
        }
    }
}