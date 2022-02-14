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
        [SerializeField]
        [Header("障害物チェックのコンポーネントを設定する")]
        ForwardObstacleCheckView _forwardObstacleCheckView;
        #endregion

        #region//フィールド
        //走行
        RunState _runState;
        RunStrategy _runStrategy;
        //追跡
        TrackState _trackState;
        TrackStrategy _trackStrategy;
        //方向転換
        ChangingDirectionStrategy _changingDirectionStrategy;
        #endregion

        #region//プロパティ
        public int Damage => _powerModel.Power.Value;
        #endregion

        // Start is called before the first frame update
        new void Awake()
        {
            base.Awake();
            _runState = GetComponent<RunState>();
            _runStrategy = GetComponent<RunStrategy>();
            _trackState = GetComponent<TrackState>();
            _trackStrategy = GetComponent<TrackStrategy>();
            _changingDirectionStrategy = GetComponent<ChangingDirectionStrategy>();
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
            _runState.DelAction = _runStrategy.Strategy;
            _trackState.DelAction = _trackStrategy.Strategy;
            DefaultState();
            Bind();
        }

        void Bind()
        {
            //前方の衝突を監視
            _forwardObstacleCheckView.IsOn
                .Where(isOn => isOn == true
                && (_actionView.HasStateBy(StateType.TRACK) == false)
                )
                .Subscribe(_ => {
                    //方向転換
                    _changingDirectionStrategy.Strategy();
                    _forwardObstacleCheckView.SetIsOn(false);
                })
                .AddTo(this);

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
        public override void CheckCollision(Collision collision)
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
        /// プレイヤーの武器に接触したか
        /// </summary>
        void CheckPlayerWeaponBy(Collider collider)
        {
            if (collider.TryGetComponent(out IPlayerWeapon playerWeapon))
            {
                //hpを減らす
                _hpModel.ReduceHp(playerWeapon.Power);
                ChangeStateByDamege();
            }
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
                _actionView.State.Value = _runState;
        }
    }
}
