using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace PlayerModel
{
    public interface IStateModel
    {
        IReadOnlyReactiveProperty<PlayerState> State { get; }

        /// <summary>
        /// プレイヤーの状態を設定します
        /// </summary>
        /// <param name="state"></param>
        void SetState(PlayerState state);
    }
}
