namespace StateView
{
    public class WaitState : StateView
    {
        public override StateType State
        {
            get => StateType.WAIT;
            set => State = value;
        }
    }
}