using UniRx;

namespace TimeModel
{
    public interface ITimeModel
    {
        IReadOnlyReactiveProperty<int> Time { get; }

        /// <summary>
        /// タイムのセット
        /// </summary>
        /// <param name="time"></param>
        void SetTime(int time);
    }
}