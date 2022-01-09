namespace PlayerView
{
    public class WaitView : StateView
    {
        public override PlayerState State
        {
            get => PlayerState.WAIT;
            set => State = value;
        }
    }
}