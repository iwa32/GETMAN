using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EP = EnemyPresenter;
using Zenject;
using System;

namespace ObjectPool
{
    public struct PoolData
    {
        public EnemyData _enemyData;
        public List<EP.EnemyPresenter> _enemyList;
    }

    public class EnemyPool : IEnemyPool
    {
        PoolData[] _enemyPools;

        [Inject]
        DiContainer container;//動的生成したデータにDIできるようにする

        /// <summary>
        /// オブジェクトプールを作成する
        /// </summary>
        /// <param name="enemyData"></param>
        /// <param name="maxEnemyCount"></param>
        public void CreatePool (EnemyData[] enemyData, int maxEnemyCount)
        {
            //enemyの種類分Poolを作成する
            _enemyPools = new PoolData[enemyData.Length];

            for (int i = 0; i < enemyData.Length; i++)
            {
                _enemyPools[i]._enemyList = new List<EP.EnemyPresenter>();
                _enemyPools[i]._enemyData = enemyData[i];

                //それぞれのenemyを最大出現数分作成しpoolします
                for (int j = 0; j < maxEnemyCount; j++)
                {
                    EP.EnemyPresenter enemy
                        = Create(enemyData[i]);

                    _enemyPools[i]._enemyList.Add(enemy);
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

            foreach (EP.EnemyPresenter enemy in GetRandomEnemyPoolData()._enemyList)
            {
                if (enemy.gameObject.activeSelf)
                {
                    continue;
                }

                enemy.gameObject?.SetActive(true);

                //パラメータの設定
                enemy.Initialize(
                    _enemyPools[0]._enemyData.Hp,
                    _enemyPools[0]._enemyData.Power,
                    _enemyPools[0]._enemyData.Speed,
                    _enemyPools[0]._enemyData.Score
                );

                return enemy;
            }

            //初期値と死亡フラグをリセット
            return null;
        }

        /// <summary>
        /// エネミーの作成
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        EP.EnemyPresenter Create(EnemyData data)
        {
            EP.EnemyPresenter enemy
                = container.InstantiatePrefab(
                            data.EnemyPrefab,
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
        PoolData GetRandomEnemyPoolData()
        {
            //エネミーのプールが複数存在するならランダムに取得します
            if (_enemyPools.Length > 1)
                return _enemyPools[UnityEngine.Random.Range(0, _enemyPools.Length)];
            else
                return _enemyPools[0];
        }
    }
}