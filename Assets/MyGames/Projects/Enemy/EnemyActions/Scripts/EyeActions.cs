using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StrategyView;

namespace EnemyActions
{
    public class EyeActions : EnemyCommonActions
    {
        //追跡
        TrackStrategy _trackStrategy;

        //todo プレイヤー方向を向く処理にする
        public TrackStrategy TrackStrategy => _trackStrategy;

        /// <summary>
        /// プレハブのインスタンス直後の処理
        /// </summary>
        public new void ManualAwake()
        {
            _trackStrategy = GetComponent<TrackStrategy>();
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
        }
    }
}