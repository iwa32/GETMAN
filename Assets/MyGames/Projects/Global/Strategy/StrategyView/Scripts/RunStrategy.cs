using System.Collections;
using System.Collections.Generic;
using Strategy;
using UnityEngine;

namespace StrategyView
{
    public abstract class RunStrategy : MonoBehaviour, IStrategy
    {
        public abstract void Strategy();
    }
}
