using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace EnemyFactory
{
    public class EnemyFactory : MonoBehaviour
    {
        [SerializeField]
        [Header("エネミーのスクリプタブルオブジェクトを設定")]
        EnemyDataList _enemyDataList;

        EnemyData[] _enemyDataToCreate;//作成エネミーデータ

        [Inject]
        DiContainer container;//動的生成したデータにDIできるようにする


        /// <summary>
        /// エネミーのデータを設定します
        /// </summary>
        /// <param name="enemyTypes"></param>
        public void SetEnemyDataByType(EnemyType[] enemyTypes)
        {
            _enemyDataToCreate
                = _enemyDataList.GetEnemyDataList
                .Where(data => enemyTypes.Contains(data.EnemyType))
                .ToArray();
        }

        /// <summary>
        /// エネミーを生成します
        /// </summary>
        public EnemyPresenter.EnemyPresenter Create()
        {
            //エネミーのデータを元に生成します
            EnemyData enemyData = GetCreatedTargetEnemyData();
            if (enemyData?.EnemyPrefab == null) return null;

            EnemyPresenter.EnemyPresenter enemy =
                container.InstantiatePrefab(
                    enemyData.EnemyPrefab,
                    Vector3.zero, Quaternion.identity, null
                )
                .GetComponent<EnemyPresenter.EnemyPresenter>();
            //パラメータの設定
            enemy.Initialize(enemyData.Hp, enemyData.Power, enemyData.Speed, enemyData.Score);

            return enemy;
        }

        /// <summary>
        /// 生成するエネミーデータを取得します
        /// </summary>
        /// <returns></returns>
        EnemyData GetCreatedTargetEnemyData()
        {
            //データが複数ならランダムに1つ選択する
            if (_enemyDataToCreate.Count() > 1)
                return _enemyDataToCreate[Random.Range(0, _enemyDataToCreate.Count())];
            else
                return _enemyDataToCreate[0];
        }
    }

}