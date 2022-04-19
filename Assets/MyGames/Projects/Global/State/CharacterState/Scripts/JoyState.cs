using System;

namespace CharacterState
{
    public class JoyState : ICharacterJoyState
    {
        public StateType State
        {
            get => StateType.JOY;
            set => State = value;
        }

        public Action DelAction { get; set; }
    }
}