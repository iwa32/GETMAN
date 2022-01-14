namespace StateView
{
    public class DeadView : StateView
    {
        public override StateType State
        {
            get => StateType.DEAD;
            set => State = value;
        }
    }
}