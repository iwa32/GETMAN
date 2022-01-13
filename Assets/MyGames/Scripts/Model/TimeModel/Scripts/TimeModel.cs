using System.Collections;
using System.Collections.Generic;
using UniRx;

namespace TimeModel
{
    public class TimeModel : ITimeModel
    {
        IntReactiveProperty _time = new IntReactiveProperty();

        public IReadOnlyReactiveProperty<int> Time => _time;

        public void SetTime(int time)
        {
            _time.Value = time;
        }
    }
}