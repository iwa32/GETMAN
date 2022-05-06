using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace ObjectPool
{
    public abstract class ObjectPool
    {
        [Inject]
        protected DiContainer container;//動的生成したデータにDIできるようにする

        /// <summary>
        /// リストからビヘイビアを取得します
        /// </summary>
        /// <param name="targetBehaviourList"></param>
        /// <returns></returns>
        protected MonoBehaviour GetBehaviourByList(IEnumerable<MonoBehaviour> targetBehaviourList)
        {
            foreach (MonoBehaviour target in targetBehaviourList)
            {
                if (target.gameObject.activeSelf)
                {
                    continue;
                }

                target.gameObject?.SetActive(true);
                return target;
            }

            return null;
        }
    }
}
