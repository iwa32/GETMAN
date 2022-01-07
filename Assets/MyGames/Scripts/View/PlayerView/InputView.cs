using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UniRx;

namespace PlayerView
{
    public class InputView : MonoBehaviour
    {
        public IReadOnlyReactiveProperty<Vector2> InputDirection => _inputDirection;

        ReactiveProperty<Vector2> _inputDirection = new ReactiveProperty<Vector2>();

        public void OnMove(InputAction.CallbackContext context)
        {
            _inputDirection.Value = context.ReadValue<Vector2>();
        }
    }
}
