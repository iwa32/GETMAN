namespace StateView
{
    public class JoyState : StateView
    {
        public override StateType State
        {
            get => StateType.JOY;
            set => State = value;
        }
    }
}