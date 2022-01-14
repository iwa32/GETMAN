namespace StateView
{
    public class RunView : StateView
    {
        public override StateType State {
            get => StateType.RUN;
            set => State = value;
        }
    }
}