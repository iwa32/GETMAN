using System;

namespace CharacterState
{
    public class DeadState : ICharacterDeadState
    {
        public StateType State
        {
            get => StateType.DEAD;
            set => State = value;
        }

        public Action DelAction { get; set; }
    }
}