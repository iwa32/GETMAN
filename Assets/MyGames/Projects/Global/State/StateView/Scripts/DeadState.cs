namespace StateView
{
    public class DeadState : StateView
    {
        public override StateType State
        {
            get => StateType.DEAD;
            set => State = value;
        }
    }
}