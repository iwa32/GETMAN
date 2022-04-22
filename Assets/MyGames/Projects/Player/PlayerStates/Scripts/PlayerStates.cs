using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Zenject;
using UniRx;
using static StateType;
using PlayerActions;
using CharacterState;
using UniRx.Triggers;
using GameModel;

namespace PlayerStates
{
    /// <summary>
    /// プレイヤーの状態を管理します
    /// </summary>
    public class PlayerStates : MonoBehaviour
    {
        //攻撃のアニメーションのステート名
        readonly string[] attackAnimStateNames
            = { "Attack1", "Attack2", "Attack3", "Attack4", "Attack5" };

        readonly string downAnimeStateName = "Down";
        //連続攻撃
        readonly string continuousAttackStateName = "ContinuousAttack";



        PlayerActions.PlayerActions _playerActions;//プレイヤーの実行処理スクリプト
        StateActionView _actionView;//プレイヤーのアクション用スクリプト
        Animator _animator;
        ObservableStateMachineTrigger _animTrigger;
        IDirectionModel _directionModel;
        //ステート
        ICharacterWaitState _waitState;//待機状態のスクリプト
        ICharacterRunState _runState;//移動状態のスクリプト
        ICharacterDownState _downState;//ダウン状態のスクリプト
        ICharacterDeadState _deadState;//デッド状態のスクリプト
        ICharacterAttackState _attackState;//攻撃状態のスクリプト
        ICharacterJoyState _joyState;//喜び状態のスクリプト

        [Inject]
        public void Construct(
            IDirectionModel direction,
            ICharacterWaitState waitState,
            ICharacterRunState runState,
            ICharacterDownState downState,
            ICharacterDeadState deadState,
            ICharacterAttackState attackState,
            ICharacterJoyState joyState
        )
        {
            _directionModel = direction;
            //ステート
            _waitState = waitState;
            _runState = runState;
            _downState = downState;
            _deadState = deadState;
            _attackState = attackState;
            _joyState = joyState;
        }

        /// <summary>
        /// プレハブのインスタンス直後の処理
        /// </summary>
        public void ManualAwake()
        {
            _playerActions = GetComponent<PlayerActions.PlayerActions>();
            _actionView = GetComponent<StateActionView>();
            _animator = GetComponent<Animator>();
            _animTrigger = _animator.GetBehaviour<ObservableStateMachineTrigger>();

        }

        public void Initialize()
        {
            InitializeState();
            Bind();
        }

        /// <summary>
        /// fixedUpdate処理
        /// </summary>
        public void ManualFixedUpdate()
        {
            _actionView.Action();
        }

        void Bind()
        {
            //viewの監視
            //状態の監視
            _actionView.State
                .Where(x => x != null)
                .Subscribe(x => _actionView.ChangeState(x.State))
                .AddTo(this);

            //model
            //ゲームの進行に応じた状態変化
            _directionModel.IsGameOver
                .Where(isGameOver => isGameOver == true)
                .Subscribe(_ => ChangeDead())
                .AddTo(this);

            _directionModel.IsGameClear
                .Where(isGameClear => isGameClear == true)
                .Subscribe(_ => ChangeJoy())
                .AddTo(this);

            //アニメーションの監視
            //攻撃
            _animTrigger.OnStateEnterAsObservable()
                .Where(s => _actionView.HasStateBy(ATTACK))
                .Where(s => IsAttackingState(s.StateInfo))
                .Subscribe(_ => _playerActions.DoNormalAttack());

            _animTrigger.OnStateExitAsObservable()
                .Where(s => IsAttackingState(s.StateInfo))
                .Subscribe(_ =>
                {
                    _animator.ResetTrigger(continuousAttackStateName);

                    if (_actionView.HasStateBy(ATTACK))
                        _actionView.State.Value = _waitState;
                });

            //down
            _animTrigger.OnStateExitAsObservable()
                .Where(s => s.StateInfo.IsName(downAnimeStateName))
                .Subscribe(_ =>
                {
                    _actionView.State.Value = _waitState;
                })
                .AddTo(this);
        }

        /// <summary>
        /// 攻撃状態か
        /// </summary>
        /// <returns></returns>
        bool IsAttackingState(AnimatorStateInfo stateInfo)
        {
            foreach (string attackStateName in attackAnimStateNames)
            {
                if (stateInfo.IsName(attackStateName)) return true;
            }

            return false;
        }

        /// <summary>
        /// 状態の初期化を行います
        /// </summary>
        void InitializeState()
        {
            _runState.DelAction = _playerActions.Run;
            _actionView.State.Value = _waitState;
        }

        /// <summary>
        /// 状態をリセットします
        /// </summary>
        public void ResetState()
        {
            _actionView.State.Value = _waitState;
        }

        /// <summary>
        /// 操作可能な状態か
        /// </summary>
        public bool IsControllableState()
        {
            return (_actionView.HasStateBy(RUN)
                || _actionView.HasStateBy(WAIT)
                || _actionView.HasStateBy(ATTACK));
        }

        /// <summary>
        /// 攻撃状態に切り替えます
        /// </summary>
        public void ChangeAttack()
        {
            //連続攻撃
            if (_actionView.HasStateBy(ATTACK))
            {
                _animator.SetTrigger(continuousAttackStateName);
            }
            _actionView.State.Value = _attackState;

        }

        public bool HasStateBy(StateType state)
        {
            return _actionView.HasStateBy(ATTACK);
        }

        /// <summary>
        /// 入力の有無でプレイヤーの状態を切り替えます
        /// </summary>
        /// <param name="input"></param>
        public void ChangeStateByInput(Vector2 input)
        {
            if (input.magnitude != 0)
                _actionView.State.Value = _runState;
            else
                _actionView.State.Value = _waitState;
        }

        /// <summary>
        /// ダメージによってプレイヤーの状態を切り替えます
        /// </summary>
        public void ChangeStateByDamage(int hp)
        {
            if (hp > 0)
                ChangeDown();
            else ChangeDead();
        }

        void ChangeDown()
        {
            _actionView.State.Value = _downState;
            _playerActions.PlayerBlinks().Forget();//点滅処理
        }

        public void ChangeDead()
        {
            _actionView.State.Value = _deadState;
            _directionModel.SetIsGameOver(true);
        }

        public void ChangeJoy()
        {
            _actionView.State.Value = _joyState;
            //カメラの方を向きます
            _playerActions.LookAtCamera();
        }
    }
}
