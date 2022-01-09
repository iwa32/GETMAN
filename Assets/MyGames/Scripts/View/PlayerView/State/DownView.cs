namespace PlayerView
{
    public class DownView : StateView
    {
        public override PlayerState State
        {
            get => PlayerState.DOWN;
            set => State = value;
        }
    }
}