using System.Collections;
using System.Collections.Generic;
using SpPlayerWeapon;
using UnityEngine;
using SPW = SpPlayerWeapon.SpPlayerWeapon;


//ジェネリックでオブジェクトプールの処理を共通化したかったが断念
namespace PlayerWeaponPool{
    /// <summary>
    /// SP武器用オブジェクトプール
    /// </summary>
    public interface ISpPlayerWeaponPool
    {
        public Dictionary<SpWeaponType, List<SPW>> SpWeaponList { get; }

        /// <summary>
        /// オブジェクトプールを作成する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prefab"></param>
        /// <param name="maxObjectCount"></param>
        public void CreatePool(SPW prefab, int maxObjectCount);

        /// <summary>
        /// プールを取得します
        /// </summary>
        /// <returns></returns>
        SPW GetPool(SpWeaponType type);
    }
}
