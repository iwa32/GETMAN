using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EP = EnemyPresenter;
using Zenject;

namespace ObjectPool
{
    public class EnemyPool : IEnemyPool
    {
        List<EP.EnemyPresenter>[] _enemyPools;

        [Inject]
        DiContainer container;//動的生成したデータにDIできるようにする

        /// <summary>
        /// オブジェクトプールを作成する
        /// </summary>
        /// <param name="enemyData"></param>
        /// <param name="maxEnemyCount"></param>
        public void CreatePool (EP.EnemyPresenter[] enemies, int maxEnemyCount)
        {
            //enemyの種類分Poolを作成する
            _enemyPools = new List<EP.EnemyPresenter>[enemies.Length];

            for (int i = 0; i < enemies.Length; i++)
            {
                _enemyPools[i] = new List<EP.EnemyPresenter>();
                //それぞれのenemyを最大出現数分作成しpoolします
                for (int j = 0; j < maxEnemyCount; j++)
                {
                    EP.EnemyPresenter enemy
                        = Create(enemies[i]);

                    _enemyPools[i].Add(enemy);
                    enemy.gameObject?.SetActive(false);
                }
            }
        }

        /// <summary>
        /// プールを取得します
        /// </summary>
        /// <returns></returns>
        public EP.EnemyPresenter GetPool()
        {
            List<EP.EnemyPresenter> poolEnemyList = GetRandomPoolEnemyList();
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

        /// <summary>
        /// エネミープールをランダムに取得します
        /// </summary>
        /// <returns></returns>
        List<EP.EnemyPresenter> GetRandomPoolEnemyList()
        {
            //エネミーのプールが複数存在するならランダムに取得します
            if (_enemyPools.Length > 1)
                return _enemyPools[Random.Range(0, _enemyPools.Length)];
            else
                return _enemyPools[0];
        }
    }
}