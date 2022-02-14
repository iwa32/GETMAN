namespace StateView
{
    public class AttackState : StateView
    {
        public override StateType State
        {
            get => StateType.ATTACK;
            set => State = value;
        }
    }
}