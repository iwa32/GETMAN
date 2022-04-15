using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourFactory
{
    public abstract class BehaviourFactory<T> : MonoBehaviour where T: class
    {
        /// <summary>
        /// Tを生成する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public abstract T Create(T prefab = null);
    }
}