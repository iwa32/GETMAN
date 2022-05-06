using System.Collections;
using System.Collections.Generic;
using SpPlayerWeapon;
using UnityEngine;
using SPW = SpPlayerWeapon.SpPlayerWeapon;
using OP = ObjectPool;

namespace PlayerWeaponPool
{
    public class SpWeaponPool: OP.ObjectPool,
        ISpPlayerWeaponPool
    {
        Dictionary<SpWeaponType, List<SPW>> _spWeaponList
            = new Dictionary<SpWeaponType, List<SPW>>();

        public Dictionary<SpWeaponType, List<SPW>> SpWeaponList => _spWeaponList;

        /// <summary>
        /// オブジェクトプールを作成する
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="maxObjectCount"></param>
        public void CreatePool(SPW prefab, int maxObjectCount)
        {
            _spWeaponList[prefab.Type] = new List<SPW>();
            for (int i = 0; i < maxObjectCount; i++)
            {
                //プレハブを生成
                SPW spWeapon = container.InstantiatePrefab(prefab).GetComponent<SPW>();
                _spWeaponList[prefab.Type].Add(spWeapon);
                spWeapon.gameObject?.SetActive(false);
            }
        }

        /// <summary>
        /// プールを取得します
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public SPW GetPool(SpWeaponType type)
        {
            return GetBehaviourByList(_spWeaponList[type])
                ?.GetComponent<SPW>();
        }
    }
}