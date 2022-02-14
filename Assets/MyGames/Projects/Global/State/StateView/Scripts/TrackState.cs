namespace StateView
{
    public class TrackState : StateView
    {
        public override StateType State
        {
            get => StateType.TRACK;
            set => State = value;
        }
    }
}