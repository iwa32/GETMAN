using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace StrategyView
{
    /// <summary>
    /// navMeshで前方に移動します
    /// </summary>
    public class RunningForwardNavmeshStrategy : RunStrategy
    {
        NavMeshAgent _navMeshAgent;

        void Awake()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
        }

        public override void Strategy()
        {
            _navMeshAgent.SetDestination(transform.position + transform.forward);
        }
    }
}
