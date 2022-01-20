namespace StateView
{
    public class JoyView : StateView
    {
        public override StateType State
        {
            get => StateType.JOY;
            set => State = value;
        }
    }
}