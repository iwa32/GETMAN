using System.Collections;
using System.Collections.Generic;
using Strategy;
using UnityEngine;

namespace StrategyView
{
    /// <summary>
    /// 走行処理を実装する
    /// </summary>
    public abstract class RunStrategy : MonoBehaviour, IStrategy
    {
        public abstract void Strategy();
    }
}
