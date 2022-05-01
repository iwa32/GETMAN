using System.Collections;
using System.Collections.Generic;
using CharacterState;
using UnityEngine;
using Zenject;
using UniRx;
using GameModel;
using EnemyActions;
using Cysharp.Threading.Tasks;

namespace EnemyStates
{
    public class SlimeStates : EnemyCommonStates
    {
        //追跡
        ICharacterTrackState _trackState;
        ICharacterDownState _downState;//ダウン状態のスクリプト
        IDirectionModel _directionModel;
        bool _isDown;

        SlimeActions _slimeActions;

        public bool IsDown => _isDown;

        [Inject]
        public void Construct(
            ICharacterTrackState trackState,
            ICharacterDownState downState,
            IDirectionModel direction
        )
        {
            _trackState = trackState;
            _downState = downState;
            _directionModel = direction;
        }

        /// <summary>
        /// プレハブのインスタンス直後の処理
        /// </summary>
        public new void ManualAwake()
        {
            _slimeActions = GetComponent<SlimeActions>();
        }

        public new void Initialize()
        {
            _runState.DelAction = _slimeActions.PatrolStrategy.Strategy;
            _trackState.DelAction = _slimeActions.TrackStrategy.Strategy;
            Bind();
        }

        void Bind()
        {
            //状態---
            //プレイヤーの追跡
            _slimeActions.TrackStrategy.CanTrack
                .TakeUntil(_isDead.Where(isDead => isDead))
                .Where(_ => _directionModel.CanGame())
                .Subscribe(canTrack => CheckTracking(canTrack))
                .AddTo(this);

            //アニメーションの監視
            //down
            _animTrigger.OnStateExitAsObservable()
                .TakeUntil(_isDead.Where(isDead => isDead))
                .Where(s => s.StateInfo.IsName("Down"))
                .Subscribe(_ => DefaultState())
                .AddTo(this);
        }

        /// <summary>
        /// 初期時、通常時の状態を設定します
        /// </summary>
        public override void DefaultState()
        {
            //巡回場所がない場合waitにする
            if (_slimeActions.PatrolStrategy.PatrolPoints?.Length == 0)
            {
                _actionView.State.Value = _waitState;
                return;
            }

            _actionView.State.Value = _runState;
        }

        /// <summary>
        /// ダメージによって状態を切り替えます
        /// </summary>
        public override void ChangeStateByDamege(int hp)
        {
            if (_isDown) return;
            if (hp > 0)
                ChangeDown();
            else
                ChangeDead();
        }

        void ChangeDown()
        {
            _isDown = true;
            _actionView.State.Value = _downState;
            ResetDown().Forget();
        }

        async UniTask ResetDown()
        {
            await UniTask.Yield();
            _isDown = false;
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
