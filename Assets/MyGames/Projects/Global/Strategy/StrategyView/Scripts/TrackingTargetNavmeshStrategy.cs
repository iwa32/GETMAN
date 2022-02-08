using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace StrategyView
{
    /// <summary>
    /// navMeshでターゲットを追跡します
    /// </summary>
    public class TrackingTargetNavmeshStrategy : TrackStrategy
    {
        NavMeshAgent _navMeshAgent;

        void Awake()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
        }

        public override void Strategy()
        {
            _navMeshAgent.SetDestination(_trackingAreaView.TargetPosition);
        }
    }
}
