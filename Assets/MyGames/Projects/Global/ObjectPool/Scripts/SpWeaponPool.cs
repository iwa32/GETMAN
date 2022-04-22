using System.Collections;
using System.Collections.Generic;
using SpPlayerWeapon;
using UnityEngine;
using SPW = SpPlayerWeapon.SpPlayerWeapon;
using Zenject;

namespace ObjectPool
{
    //public struct SpWeaponPoolData
    //{
    //    public SpWeaponType _type;
    //    public List<GameObject> _spWeaponList;
    //}

    public class SpWeaponPool: ISpPlayerWeaponPool
    {
        List<SPW> _spWeaponList = new List<SPW>();

        public List<SPW> SpWeaponList => _spWeaponList;

        [Inject]
        DiContainer container;//動的生成したデータにDIできるようにする

        /// <summary>
        /// オブジェクトプールを作成する
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="maxObjectCount"></param>
        public void CreatePool(SPW prefab, int maxObjectCount)
        {
            for (int i = 0; i < maxObjectCount; i++)
            {
                //プレハブを生成
                SPW spWeapon = container.InstantiatePrefab(prefab).GetComponent<SPW>();
                _spWeaponList.Add(spWeapon);
                spWeapon.gameObject?.SetActive(false);
            }
        }

        /// <summary>
        /// プールを取得します
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public ISpPlayerWeapon GetPool(Vector3 pos, Quaternion rotation)
        {
            foreach (SPW spWeapon in _spWeaponList)
            {
                if (spWeapon.gameObject.activeSelf)
                {
                    continue;
                }
                spWeapon.transform.position = pos;
                spWeapon.transform.rotation = rotation;
                spWeapon.gameObject?.SetActive(true);

                return spWeapon?.GetComponent<ISpPlayerWeapon>();
            }
            return null;
        }
    }
}