using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StrategyView
{
    public class TrackingByEyeStrategy : TrackStrategy
    {
        public override void Strategy()
        {
            transform.LookAt(_trackingAreaView.TargetTransform);
        }
    }
}