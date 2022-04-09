using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using EP = EnemyPresenter;
using ObjectPool;

namespace BehaviourFactory
{
    public class EnemyFactory : BehaviourFactory<EP.EnemyPresenter>
    {
        [SerializeField]
        [Header("エネミーのスクリプタブルオブジェクトを設定")]
        EnemyDataList _enemyDataList;

        EnemyData[] _enemyDataToCreate;//作成エネミーデータ
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
        public void SetEnemyData(EnemyType[] enemyTypes, int maxEnemyCount)
        {
            //エネミーデータからステージに出現するエネミーを探し格納します
            _enemyDataToCreate
                = _enemyDataList.GetEnemyDataList
                .Where(data => enemyTypes.Contains(data.EnemyType))
                .ToArray();

            _enemyPool.CreatePool(_enemyDataToCreate, maxEnemyCount);
        }

        /// <summary>
        /// エネミーを生成します
        /// </summary>
        public override EP.EnemyPresenter Create()
        {
            //poolから取得します
            return _enemyPool.GetPool();
        }
    }

}