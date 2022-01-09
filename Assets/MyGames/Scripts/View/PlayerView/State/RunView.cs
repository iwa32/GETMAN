namespace PlayerView
{
    public class RunView : StateView
    {
        public override PlayerState State {
            get => PlayerState.RUN;
            set => State = value;
        }
    }
}