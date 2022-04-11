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
        public IReadOnlyReactiveProperty<bool> IsSpAttack => _isSpAttack;

        ReactiveProperty<Vector2> _inputDirection = new ReactiveProperty<Vector2>();
        ReactiveProperty<bool> _isFired = new ReactiveProperty<bool>();
        ReactiveProperty<bool> _isSpAttack = new ReactiveProperty<bool>();

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
        public void OnFire(InputAction.CallbackContext context)
        {
            _isFired.Value = CheckInputActionPhase(context);
        }

        /// <summary>
        /// 発火入力
        /// </summary>
        public void OnSpAttack(InputAction.CallbackContext context)
        {
            _isSpAttack.Value = CheckInputActionPhase(context);
        }

        bool CheckInputActionPhase(InputAction.CallbackContext context)
        {
            //入力時にフラグをオンにする
            if (context.phase == InputActionPhase.Started)
                return true;

            //if (context.phase == InputActionPhase.Performed)
            //    return false;
            return false;
        }
    }
}
