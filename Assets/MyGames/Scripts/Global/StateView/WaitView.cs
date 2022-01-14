namespace StateView
{
    public class WaitView : StateView
    {
        public override StateType State
        {
            get => StateType.WAIT;
            set => State = value;
        }
    }
}