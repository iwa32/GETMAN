using System;

namespace CharacterState
{
    public class AttackState: ICharacterAttackState
    {
        public StateType State
        {
            get => StateType.ATTACK;
            set => State = value;
        }

        public Action DelAction { get; set; }
    }
}