namespace PlayerView
{
    public class AttackView : StateView
    {
        public override PlayerState State
        {
            get => PlayerState.ATTACK;
            set => State = value;
        }
    }
}