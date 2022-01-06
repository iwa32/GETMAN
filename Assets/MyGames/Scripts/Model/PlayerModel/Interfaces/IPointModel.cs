using UniRx;

namespace PlayerModel
{
    public interface IPointModel
    {
        IReadOnlyReactiveProperty<int> Point { get; }

        /// <summary>
        /// ポイントの増加
        /// </summary>
        /// <param name="point"></param>
        void AddPoint(int point);

        /// <summary>
        /// ポイントのリセット
        /// </summary>
        void ResetPoint();
    }
}
