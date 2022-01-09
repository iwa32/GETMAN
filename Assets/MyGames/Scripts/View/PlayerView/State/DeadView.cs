namespace PlayerView
{
    public class DeadView : StateView
    {
        public override PlayerState State
        {
            get => PlayerState.DEAD;
            set => State = value;
        }
    }
}