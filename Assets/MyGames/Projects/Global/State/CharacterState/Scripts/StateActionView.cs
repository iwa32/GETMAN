using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace CharacterState
{
    /// <summary>
    /// 各Stateの管理、呼び出し用スクリプト
    /// </summary>
    public class StateActionView : MonoBehaviour
    {
        Animator _animator;

        public ReactiveProperty<ICharacterState> State = new ReactiveProperty<ICharacterState>();

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
            if (State.Value?.DelAction == null) return;
            State.Value.DelAction();
        }

        /// <summary>
        /// Stateの状態であればtrue
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public bool HasStateBy(StateType state)
        {
            return (State.Value?.State == state);
        }
    }
}