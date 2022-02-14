namespace StateView
{
    public class RunState : StateView
    {
        public override StateType State {
            get => StateType.RUN;
            set => State = value;
        }
    }
}