using System;

namespace CharacterState
{
    public class WaitState : ICharacterWaitState
    {
        public StateType State
        {
            get => StateType.WAIT;
            set => State = value;
        }

        public Action DelAction { get; set; }
    }
}