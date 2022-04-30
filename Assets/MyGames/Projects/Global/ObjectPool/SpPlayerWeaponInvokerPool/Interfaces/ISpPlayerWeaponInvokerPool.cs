using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SPWI = SpPlayerWeaponInvoker;

namespace SpPlayerWeaponInvokerPool
{
    public interface ISpPlayerWeaponInvokerPool
    {
        /// <summary>
        /// オブジェクトプールを作成する
        /// </summary>
        /// <param name="invoker"></param>
        void CreatePool(SPWI.SpPlayerWeaponInvoker invoker);

        /// <summary>
        /// プールを取得します
        /// </summary>
        /// <returns></returns>
        SPWI.SpPlayerWeaponInvoker GetPool(SpWeaponType spPlayerWeaponType);
    }
}
