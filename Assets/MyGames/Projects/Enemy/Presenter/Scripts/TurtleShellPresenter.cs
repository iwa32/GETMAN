using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using StateView;
using EnemyView;
using StageObject;
using StrategyView;

namespace EnemyPresenter
{
    public class TurtleShellPresenter : EnemyPresenter, IDamager
    {
        #region//インスペクターから設定
        #endregion

        #region//フィールド
        //走行
        RunState _runState;
        //追跡
        TrackState _trackState;
        TrackStrategy _trackStrategy;
        #endregion

        #region//プロパティ
        public int Damage => _powerModel.Power.Value;
        #endregion

        // Start is called before the first frame update
        new void Awake()
        {
            base.Awake();
            _runState = GetComponent<RunState>();
            _trackState = GetComponent<TrackState>();
            _trackStrategy = GetComponent<TrackStrategy>();
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
                .Subscribe(canTrack => CheckTracking(canTrack))
                .AddTo(this);
        }

        #region //overrideMethod
        /// <summary>
        /// 接触したコライダーを確認します
        /// </summary>
        /// <param name="collider"></param>
        public override void CheckCollider(Collider collider)
        {
            //武器に接触でダメージを受ける
            CheckPlayerWeaponBy(collider);
        }

        /// <summary>
        /// 衝突を確認します
        /// </summary>
        public override void CheckCollision(UnityEngine.Collision collision)
        {

        }

        /// <summary>
        /// 初期時、通常時の状態を設定します
        /// </summary>
        public override void DefaultState()
        {
            _actionView.State.Value = _runState;
        }
        #endregion

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
                _actionView.State.Value = _runState;
        }
    }
}
