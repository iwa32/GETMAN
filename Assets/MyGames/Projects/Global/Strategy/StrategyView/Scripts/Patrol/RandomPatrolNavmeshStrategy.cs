using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace StrategyView
{
    /// <summary>
    /// ランダムな地点に巡回します
    /// </summary>
    public class RandomPatrolNavmeshStrategy : PatrolStrategy
    {
        NavMeshAgent _navMeshAgent;
        Transform[] _patrolPoints;
        Vector3 _targetPoint;

        public override Transform[] PatrolPoints => _patrolPoints;

        void Awake()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _navMeshAgent.autoBraking = false;
        }

        /// <summary>
        /// 巡回地点の設定
        /// </summary>
        /// <param name="points"></param>
        public override void SetPatrolPoints(Transform[] points)
        {
            _patrolPoints = points;
            GotoNextPoint();
        }

        public override void Strategy()
        {
            if (_navMeshAgent.pathPending == false && _navMeshAgent.remainingDistance < 0.1f)
            {
                GotoNextPoint();
            }
        }

        /// <summary>
        /// 次の巡回場所へ
        /// </summary>
        void GotoNextPoint()
        {
            if (_patrolPoints.Length == 0) return;

            _targetPoint = GetPatrolPoint();
            _navMeshAgent.SetDestination(_targetPoint);
        }

        Vector3 GetPatrolPoint()
        {
            Vector3 nextPoint = GetRandomPatrolPoint();
            //前回と同じ地点を取得したら再取得します
            while (_targetPoint == GetRandomPatrolPoint())
            {
                nextPoint = GetRandomPatrolPoint();
            }

            return nextPoint;
        }

        Vector3 GetRandomPatrolPoint()
        {
            return _patrolPoints[Random.Range(0, _patrolPoints.Length)]
                .transform.position;
        }
    }
}