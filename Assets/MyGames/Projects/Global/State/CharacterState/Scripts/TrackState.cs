using System;

namespace CharacterState
{
    public class TrackState : ICharacterTrackState
    {
        public StateType State
        {
            get => StateType.TRACK;
            set => State = value;
        }

        public Action DelAction { get; set; }
    }
}