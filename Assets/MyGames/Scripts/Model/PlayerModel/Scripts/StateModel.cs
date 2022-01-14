using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace PlayerModel
{
    public class StateModel : IStateModel
    {
        public IReadOnlyReactiveProperty<StateType> State => _state;

        ReactiveProperty<StateType> _state = new ReactiveProperty<StateType>();

        public void SetState(StateType state)
        {
            _state.Value = state;
        }
    }
}
