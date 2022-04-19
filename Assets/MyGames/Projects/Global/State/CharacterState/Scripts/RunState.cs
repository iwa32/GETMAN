using System;

namespace CharacterState
{
    public class RunState : ICharacterRunState
    {
        public StateType State
        {
            get => StateType.RUN;
            set => State = value;
        }

        public Action DelAction { get; set; }
    }
}