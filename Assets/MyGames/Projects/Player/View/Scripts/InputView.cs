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
        public IReadOnlyReactiveProperty<bool> IsFired => _isFired;

        ReactiveProperty<Vector2> _inputDirection = new ReactiveProperty<Vector2>();
        ReactiveProperty<bool> _isFired = new ReactiveProperty<bool>();

        /// <summary>
        /// 移動入力
        /// </summary>
        /// <param name="context"></param>
        public void OnMove(InputAction.CallbackContext context)
        {
            _inputDirection.Value = context.ReadValue<Vector2>();
        }

        /// <summary>
        /// 発火入力
        /// </summary>
        public void OnAction(InputAction.CallbackContext context)
        {
            //入力時にフラグをオンにし、入力後にフラグをオフにする
            if (context.phase == InputActionPhase.Started)
                _isFired.Value = true;

            if (context.phase == InputActionPhase.Performed)
                _isFired.Value = false;
        }
    }
}
