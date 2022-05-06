using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SPWI = SpPlayerWeaponInvoker;
using OP = ObjectPool;

namespace SpPlayerWeaponInvokerPool
{
    public class SpPlayerWeaponInvokerPool : OP.ObjectPool,
        ISpPlayerWeaponInvokerPool
    {
        Dictionary<SpWeaponType, List<SPWI.SpPlayerWeaponInvoker>> _invokerPoolList
            = new Dictionary<SpWeaponType, List<SPWI.SpPlayerWeaponInvoker>>();

        public void CreatePool(SPWI.SpPlayerWeaponInvoker invokerPrefab)
        {
            //SP武器呼び出し用のインスタンスを作成し、プールに登録
            _invokerPoolList[invokerPrefab.Type] = new List<SPWI.SpPlayerWeaponInvoker>();
            SPWI.SpPlayerWeaponInvoker invoker = Create(invokerPrefab);
            _invokerPoolList[invoker.Type].Add(invoker);
            invoker.gameObject?.SetActive(false);
        }

        /// <summary>
        /// プールを取得します
        /// </summary>
        /// <returns></returns>
        public SPWI.SpPlayerWeaponInvoker GetPool(SpWeaponType type)
        {
            try
            {
                return GetBehaviourByList(_invokerPoolList[type])
                ?.GetComponent<SPWI.SpPlayerWeaponInvoker>();
            }
            catch
            {
                //取得時にtypeキーに紐づいたdictionaryが作成されていなかった場合
                return null;
            }
        }

        /// <summary>
        /// エネミーの武器作成
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        SPWI.SpPlayerWeaponInvoker Create(SPWI.SpPlayerWeaponInvoker prefab)
        {
            SPWI.SpPlayerWeaponInvoker invoker
                = container.InstantiatePrefab(prefab)
                .GetComponent<SPWI.SpPlayerWeaponInvoker>();

            return invoker;
        }
    }
}