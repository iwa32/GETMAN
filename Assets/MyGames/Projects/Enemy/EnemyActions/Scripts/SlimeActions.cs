using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StrategyView;

namespace EnemyActions
{
    public class SlimeActions : EnemyCommonActions
    {
        //追跡
        TrackStrategy _trackStrategy;
        //---巡回---
        PatrolStrategy _patrolStrategy;

        public TrackStrategy TrackStrategy => _trackStrategy;
        public PatrolStrategy PatrolStrategy => _patrolStrategy;

        /// <summary>
        /// プレハブのインスタンス直後の処理
        /// </summary>
        public new void ManualAwake()
        {
            _trackStrategy = GetComponent<TrackStrategy>();
            _patrolStrategy = GetComponent<PatrolStrategy>();
        }

        /// <summary>
        /// ステージ情報を設定します
        /// </summary>
        /// <param name="stageView"></param>
        public override void SetStageInformation(StageView.StageView stageView, EnemyType type)
        {
            //配置
            Transform appearancePoint = stageView.GetEnemyAppearancePoint(type);
            SetTransform(appearancePoint);
            SetPatrolPoints(stageView.GetEnemyPatrolPoints(type));
        }

        /// <summary>
        /// 巡回地点を設定します
        /// </summary>
        /// <param name="points"></param>
        void SetPatrolPoints(Transform[] points)
        {
            _patrolStrategy.SetPatrolPoints(points);
        }
    }
}
