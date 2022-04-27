using System.Collections;
using System.Collections.Generic;
using CharacterState;
using UnityEngine;
using Zenject;
using UniRx;
using GameModel;
using EnemyActions;

namespace EnemyStates
{
    public class EyeStates : EnemyCommonStates
    {
        //追跡
        ICharacterTrackState _trackState;
        IDirectionModel _directionModel;

        EyeActions _eyeActions;

        [Inject]
        public void Construct(
            ICharacterTrackState trackState,
            IDirectionModel direction
        )
        {
            _trackState = trackState;
            _directionModel = direction;
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

            //track状態が2秒以上でその方向に攻撃するように
            _actionView.State
                .TakeUntil(_isDead.Where(isDead => isDead))
                .Where(x => x.State == StateType.TRACK)
                .Subscribe(x => Debug.Log("2秒後攻撃ステートにする"))
                //攻撃ステートになったら攻撃し、その後2秒待ち、waitになる
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
    }
}