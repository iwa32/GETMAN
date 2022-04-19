using System;

namespace CharacterState
{
    public class DownState : ICharacterDownState
    {
        public StateType State
        {
            get => StateType.DOWN;
            set => State = value;
        }

        public Action DelAction { get; set; }
    }
}