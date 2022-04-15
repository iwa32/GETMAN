using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using EP = EnemyPresenter;
using EDL = EnemyDataList;
using ObjectPool;

namespace BehaviourFactory
{
    public class EnemyFactory : BehaviourFactory<EP.EnemyPresenter>
    {
        [SerializeField]
        [Header("EnemyDataListのスクリプタブルオブジェクトを設定")]
        EDL.EnemyDataList _enemyDataList;

        [SerializeField]
        [Header("BossEnemyDataListのスクリプタブルオブジェクトを設定")]
        EDL.EnemyDataList _bossEnemyDataList;

        IEnemyPool _enemyPool;

        [Inject]
        public void Construct(IEnemyPool enemyPool)
        {
            _enemyPool = enemyPool;
        }

        /// <summary>
        /// エネミーを生成します
        /// </summary>
        /// <param name="enemyTypes"></param>
        public void SetEnemyPool(EP.EnemyPresenter enemy, int maxEnemyCount = 1)
        {
            _enemyPool.CreatePool(enemy, maxEnemyCount);
        }

        /// <summary>
        /// ボスを作成します
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns></returns>
        public EP.EnemyPresenter CreateTheBoss(EP.EnemyPresenter prefab)
        {
            return InitializeEnemy(prefab.Type, _bossEnemyDataList);
        }

        /// <summary>
        /// エネミーを生成します
        /// </summary>
        public override EP.EnemyPresenter Create(EP.EnemyPresenter prefab)
        {
            return InitializeEnemy(prefab.Type, _enemyDataList);
        }

        /// <summary>
        /// エネミーの初期化をします
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        EP.EnemyPresenter InitializeEnemy(EnemyType type, EDL.EnemyDataList dataList)
        {
            //poolから取得します
            EP.EnemyPresenter enemy = _enemyPool.GetPool(type);

            if (enemy == null) return null;

            EDL.EnemyData data = dataList.GetEnemyDataList
                .First(data => data.EnemyType == type);

            enemy.Initialize(data);

            return enemy;
        }
    }

}