namespace StateView
{
    public class DownView : StateView
    {
        public override StateType State
        {
            get => StateType.DOWN;
            set => State = value;
        }
    }
}