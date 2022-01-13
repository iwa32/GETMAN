using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace PlayerModel
{
    public class StateModel : IStateModel
    {
        public IReadOnlyReactiveProperty<PlayerState> State => _state;

        ReactiveProperty<PlayerState> _state = new ReactiveProperty<PlayerState>();

        public void SetState(PlayerState state)
        {
            _state.Value = state;
        }
    }
}
