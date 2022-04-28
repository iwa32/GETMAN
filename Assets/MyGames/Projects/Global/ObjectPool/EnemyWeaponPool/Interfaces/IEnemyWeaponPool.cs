using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyWeapon;

namespace EnemyWeaponPool
{
    public interface IEnemyWeaponPool
    {
        /// <summary>
        /// オブジェクトプールを作成する
        /// </summary>
        /// <param name="enemyWeapon"></param>
        /// <param name="maxObjectCount"></param>
        void CreatePool(EnemyWeapon.EnemyWeapon enemyWeapon, int maxObjectCount);

        /// <summary>
        /// プールを取得します
        /// </summary>
        /// <returns></returns>
        EnemyWeapon.EnemyWeapon GetPool();
    }
}
