namespace StateView
{
    public class TrackView : StateView
    {
        public override StateType State
        {
            get => StateType.TRACK;
            set => State = value;
        }
    }
}