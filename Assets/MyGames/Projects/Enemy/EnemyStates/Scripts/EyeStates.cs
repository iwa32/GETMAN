using System.Collections;
using System.Collections.Generic;
using CharacterState;
using UnityEngine;
using Zenject;
using Cysharp.Threading.Tasks;
using UniRx;
using EnemyActions;
using System;

namespace EnemyStates
{
    public class EyeStates : EnemyCommonStates
    {
        readonly string lastAttackMotionAnimStateName = "AttackMotion2";

        [SerializeField]
        [Header("追跡から攻撃までの時間")]
        int _timeFromTrackingToAttack = 3;

        //追跡
        ICharacterTrackState _trackState;
        ICharacterAttackState _attackState;
        

        EyeActions _eyeActions;

        [Inject]
        public void Construct(
            ICharacterTrackState trackState,
            ICharacterAttackState attackState
            
        )
        {
            _trackState = trackState;
            _attackState = attackState;
        }

        /// <summary>
        /// プレハブのインスタンス直後の処理
        /// </summary>
        public new void ManualAwake()
        {
            _eyeActions = GetComponent<EyeActions>();
        }

        public new void Initialize()
        {
            _trackState.DelAction = _eyeActions.TrackStrategy.Strategy;
            Bind();
        }

        void Bind()
        {
            //状態---
            //プレイヤーの追跡
            _eyeActions.TrackStrategy.CanTrack
                .TakeUntil(_isDead.Where(isDead => isDead))
                .Where(_ => _directionModel.CanGame())
                .Subscribe(canTrack => CheckTracking(canTrack))
                .AddTo(this);

            //追跡時に攻撃状態に移行するか
            _actionView.State
                .TakeUntil(_isDead.Where(isDead => isDead))
                .Where(_ => _directionModel.CanGame())
                .Where(x => x != null)
                .Where(x => x.State == StateType.TRACK)
                .Subscribe(x => ChangeAttackAsync().Forget())
                .AddTo(this);

            //攻撃アニメーション時、レーザーを出す
            _animTrigger.OnStateEnterAsObservable()
                .TakeUntil(_isDead.Where(isDead => isDead))
                .Where(s => s.StateInfo.IsName(lastAttackMotionAnimStateName))
                .Subscribe(_ => _eyeActions.Attack())
                .AddTo(this);

            //攻撃後、追跡状態を確認
            _animTrigger.OnStateExitAsObservable()
                .TakeUntil(_isDead.Where(isDead => isDead))
                .Where(s => s.StateInfo.IsName(lastAttackMotionAnimStateName))
                .Subscribe(_ =>
                {
                    CheckTracking(_eyeActions.TrackStrategy.CanTrack.Value);
                })
                .AddTo(this);

        }

        public override void DefaultState()
        {
            _actionView.State.Value = _waitState;
        }

        /// <summary>
        /// 追跡の確認をします
        /// </summary>
        /// <param name="canTrack"></param>
        void CheckTracking(bool canTrack)
        {
            //追跡もしくは待機状態になります
            if (canTrack)
                _actionView.State.Value = _trackState;
            else
                DefaultState();
        }

        /// <summary>
        /// 攻撃状態にします
        /// </summary>
        async UniTask ChangeAttackAsync()
        {
            //指定時間後も追跡しているなら攻撃状態にする
            await UniTask.Delay(TimeSpan.FromSeconds(_timeFromTrackingToAttack));
            if (_actionView.HasStateBy(StateType.TRACK) == false) return;

            _actionView.State.Value = _attackState;
        }

        public override void ChangeStateByDamege(int hp)
        {
            if (hp <= 0)
                ChangeDead();
        }
    }
}