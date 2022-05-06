using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EW = EnemyWeapon;
using OP = ObjectPool;

namespace EnemyWeaponPool
{
    public class EnemyWeaponPool : OP.ObjectPool,
        IEnemyWeaponPool
    {
        Dictionary<EnemyWeaponType, List<EW.EnemyWeapon>> _enemyWeaponPoolList
            = new Dictionary<EnemyWeaponType, List<EW.EnemyWeapon>>();

        public void CreatePool(EW.EnemyWeapon enemyWeaponPrefab, int maxObjectCount)
        {
            //対象の武器のリストを生成
            _enemyWeaponPoolList[enemyWeaponPrefab.Type] = new List<EW.EnemyWeapon>();

            for (int i = 0; i < maxObjectCount; i++)
            {
                EW.EnemyWeapon enemyWeapon = Create(enemyWeaponPrefab);

                _enemyWeaponPoolList[enemyWeapon.Type].Add(enemyWeapon);
                enemyWeapon.gameObject?.SetActive(false);
            }
        }

        /// <summary>
        /// プールを取得します
        /// </summary>
        /// <returns></returns>
        public EW.EnemyWeapon GetPool(EnemyWeaponType type)
        {
            return GetBehaviourByList(_enemyWeaponPoolList[type])
                ?.GetComponent<EW.EnemyWeapon>();
        }

        /// <summary>
        /// エネミーの武器作成
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        EW.EnemyWeapon Create(EW.EnemyWeapon prefab)
        {
            EW.EnemyWeapon enemyWeapon
                = container.InstantiatePrefab(prefab)
                .GetComponent<EW.EnemyWeapon>();

            return enemyWeapon;
        }
    }
}