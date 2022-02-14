namespace StateView
{
    public class DownState : StateView
    {
        public override StateType State
        {
            get => StateType.DOWN;
            set => State = value;
        }
    }
}