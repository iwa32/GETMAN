using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Strategy;

namespace StrategyView
{
    /// <summary>
    /// 進行方向を変えるクラス
    /// </summary>
    public abstract class ChangingDirectionStrategy : MonoBehaviour, IStrategy
    {
        public abstract void Strategy();
    }
}
