using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StrategyView;
using EWI = EnemyWeaponInvoker;

namespace EnemyActions
{
    public class EyeActions : EnemyCommonActions
    {
        //追跡
        TrackStrategy _trackStrategy;//目で追跡するストラテジークラス
        EWI.EnemyWeaponInvoker _penetrationLaserInvoker;//貫通レーザー攻撃の呼び出し用クラス

        public TrackStrategy TrackStrategy => _trackStrategy;

        /// <summary>
        /// プレハブのインスタンス直後の処理
        /// </summary>
        public new void ManualAwake()
        {
            _trackStrategy = GetComponent<TrackStrategy>();
            _penetrationLaserInvoker = GetComponent<EWI.EnemyWeaponInvoker>();
        }

        public void Initialize()
        {
            _penetrationLaserInvoker.SetPower(_enemyData.Power);
            _penetrationLaserInvoker.SetEnemyTransform(transform);
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

        public void Attack()
        {
            _penetrationLaserInvoker.Invoke();
        }
    }
}