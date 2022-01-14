using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace StateView
{
    /// <summary>
    /// 各Stateの管理、呼び出し用スクリプト
    /// </summary>
    public class ActionView : MonoBehaviour
    {
        Animator _animator;

        public ReactiveProperty<StateView> State = new ReactiveProperty<StateView>();

        void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        /// <summary>
        /// キャラクターの状態を切り替えます
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