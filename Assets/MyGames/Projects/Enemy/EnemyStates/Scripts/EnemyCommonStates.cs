using System.Collections;
using System.Collections.Generic;
using CharacterState;
using UnityEngine;
using EnemyActions;
using UniRx;
using UniRx.Triggers;
using Zenject;
using Cysharp.Threading.Tasks;

namespace EnemyStates
{
    /// <summary>
    /// エネミー共通の状態管理クラス
    /// </summary>
    public abstract class EnemyCommonStates : MonoBehaviour
    {
        Animator _animator;
        protected EnemyCommonActions _enemyCommonActions;
        protected ObservableStateMachineTrigger _animTrigger;//stateMachineの監視
        protected BoolReactiveProperty _isDead = new BoolReactiveProperty();

        //ステート
        ICharacterDeadState _deadState;//デッド状態のスクリプト
        protected StateActionView _actionView;//エネミーのアクション用スクリプト
        protected ICharacterWaitState _waitState;//待機状態のスクリプト
        protected ICharacterRunState _runState;//移動状態のスクリプト

        public IReadOnlyReactiveProperty<bool> IsDead => _isDead;

        [Inject]
        public void Construct(
            ICharacterWaitState waitState,
            ICharacterRunState runState,
            ICharacterDeadState deadState
        )
        {
            _waitState = waitState;
            _runState = runState;
            _deadState = deadState;
        }

        /// <summary>
        /// プレハブのインスタンス直後の処理
        /// </summary>
        public void ManualAwake()
        {
            _actionView = GetComponent<StateActionView>();
            _animator = GetComponent<Animator>();
            _animTrigger = _animator.GetBehaviour<ObservableStateMachineTrigger>();
            _enemyCommonActions = GetComponent<EnemyCommonActions>();
        }

        public void Initialize()
        {
            //awakeでanimeTriggerを取得した場合アニメーションの終了検知がうまくいかない場合があるため、こちらで設定する
            _animTrigger = _animator.GetBehaviour<ObservableStateMachineTrigger>();
            _isDead.Value = false;
            Bind();
        }

        void Bind()
        {
            //状態------
            //view to model
            //状態の監視
            _actionView.State
                .TakeUntil(_isDead.Where(isDead => isDead))
                .Where(x => x != null)
                //死亡アニメーション中でない場合
                .Where(x => _animator.GetInteger("States") != (int)StateType.DEAD)
                .Subscribe(x => _actionView.ChangeState(x.State))//アニメーションの切り替え
                .AddTo(this);

            //dead
            _animTrigger.OnStateExitAsObservable()
                .TakeUntil(_isDead.Where(isDead => isDead))
                .Where(s => s.StateInfo.IsName("Dead"))
                .Subscribe(_ =>
                {
                    gameObject.SetActive(false);
                    _isDead.Value = true;
                    DefaultState();
                }).AddTo(this);

            //FixedUpdate
            this.FixedUpdateAsObservable()
                .TakeUntil(_isDead.Where(isDead => isDead))
                .Subscribe(_ => _actionView.Action())
                .AddTo(this);
        }

        #region //abstractMethod
        /// <summary>
        /// 初期時、通常時の状態を設定します
        /// </summary>
        public abstract void DefaultState();

        ///// <summary>
        ///// ダメージによって状態を切り替えます
        ///// </summary>
        public abstract void ChangeStateByDamege(int hp);
        #endregion

        public bool HasStateBy(StateType state)
        {
            return _actionView.HasStateBy(state);
        }

        protected void ChangeDead()
        {
            _enemyCommonActions.Dead();
            _actionView.State.Value = _deadState;
            _enemyCommonActions.JudgeDrop();
        }
    }
}
