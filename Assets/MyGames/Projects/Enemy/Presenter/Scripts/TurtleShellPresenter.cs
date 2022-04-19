using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using StateView;
using EnemyView;
using StrategyView;
using GlobalInterface;

namespace EnemyPresenter
{
    public class TurtleShellPresenter : EnemyPresenter, IPlayerAttacker
    {
        #region//インスペクターから設定
        #endregion

        #region//フィールド
        //追跡
        TrackState _trackState;
        TrackStrategy _trackStrategy;
        //---巡回---
        PatrolStrategy _patrolStrategy;
        #endregion

        #region//プロパティ
        public int Power => _powerModel.Power.Value;
        public PatrolStrategy PatrolStrategy => _patrolStrategy;
        #endregion

        // Start is called before the first frame update
        new void Awake()
        {
            base.Awake();
            //追跡
            _trackState = GetComponent<TrackState>();
            _trackStrategy = GetComponent<TrackStrategy>();
            //巡回
            _patrolStrategy = GetComponent<PatrolStrategy>();
        }

        void Start()
        {
            Initialize();
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        public void Initialize()
        {
            _runState.DelAction = PatrolStrategy.Strategy;
            _trackState.DelAction = _trackStrategy.Strategy;
            Bind();
        }

        void Bind()
        {
            //プレイヤーの追跡
            _trackStrategy.CanTrack
                .TakeUntil(_isDead.Where(isDead => isDead))
                .Where(_ => _directionModel.CanGame())
                .Subscribe(canTrack => CheckTracking(canTrack))
                .AddTo(this);
        }

        #region //overrideMethod

        public override void CheckCollider(Collider collider)
        {
            //武器に接触でダメージを受ける
            CheckPlayerWeaponBy(collider);
        }

        public override void CheckCollision(UnityEngine.Collision collision)
        {

        }

        /// <summary>
        /// 初期時、通常時の状態を設定します
        /// </summary>
        public override void DefaultState()
        {
            //巡回場所がない場合waitにする
            if (_patrolStrategy?.PatrolPoints.Length == 0)
            {
                _actionView.State.Value = _waitState;
                return;
            }

            _actionView.State.Value = _runState;
        }
        #endregion

        //// <summary>
        /// ステージ情報を設定します
        /// </summary>
        /// <param name="stageView"></param>
        public override void SetStageInformation(StageView.StageView stageView)
        {
            //配置
            Transform appearancePoint = stageView.GetEnemyAppearancePoint(_type);
            SetTransform(appearancePoint);
            SetPatrolPoints(stageView.GetEnemyPatrolPoints(_type));
        }

        /// <summary>
        /// 巡回地点を設定します
        /// </summary>
        /// <param name="points"></param>
        void SetPatrolPoints(Transform[] points)
        {
            _patrolStrategy.SetPatrolPoints(points);
        }

        /// <summary>
        /// 追跡の確認をします
        /// </summary>
        /// <param name="canTrack"></param>
        void CheckTracking(bool canTrack)
        {
            //追跡もしくは前方を走ります
            if (canTrack)
                _actionView.State.Value = _trackState;
            else
                DefaultState();
        }
    }
}
