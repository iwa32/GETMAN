namespace StateView
{
    public class AttackView : StateView
    {
        public override StateType State
        {
            get => StateType.ATTACK;
            set => State = value;
        }
    }
}