using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace PlayerView
{
    public class ActionView : MonoBehaviour
    {
        Animator _animator;

        public ReactiveProperty<StateView> State = new ReactiveProperty<StateView>();

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
            _animator.SetInteger("States", (int)state);
        }

        public void Action()
        {
            State.Value.Action();
        }
    }
}