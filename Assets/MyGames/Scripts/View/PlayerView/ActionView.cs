using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateView;
using UniRx;

namespace PlayerView
{
    public class ActionView : MonoBehaviour
    {
        Animator _animator;

        public ReactiveProperty<StateView.StateView> State = new ReactiveProperty<StateView.StateView>();

        void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        /// <summary>
        /// プレイヤーの状態を切り替えます
        /// </summary>
        /// <param name="state"></param>
        public void ChangeState(StateType state)
        {
            _animator.SetInteger("States", (int)state);
        }

        public void Action()
        {
            if (State.Value.DelAction == null) return;
            State.Value.Action();
        }
    }
}