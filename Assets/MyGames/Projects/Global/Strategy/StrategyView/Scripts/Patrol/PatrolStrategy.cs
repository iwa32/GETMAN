using System.Collections;
using System.Collections.Generic;
using Strategy;
using UnityEngine;

namespace StrategyView
{
    /// <summary>
    /// 巡回処理を実装する
    /// </summary>
    public abstract class PatrolStrategy : MonoBehaviour, IStrategy
    {
        /// <summary>
        /// 巡回場所を設定
        /// </summary>
        /// <param name="points"></param>
        public abstract void SetPatrolPoints(Transform[] points);

        public abstract void Strategy();
    }
}