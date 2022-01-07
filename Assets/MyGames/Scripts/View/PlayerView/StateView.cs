using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace PlayerView
{
    public class StateView : MonoBehaviour
    {
        Action _delAction;
        Animator _animator;

        void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        /// <summary>
        /// プレイヤーの状態を切り替えます
        /// </summary>
        /// <param name="state"></param>
        public void ChangeState(PlayerState state)
        {
            _animator.SetInteger("States",(int)state);
        }

        public void Action()
        {
            _delAction();
        }

        public void SetDelAction(Action action)
        {
            _delAction = action;
        }
    }
}