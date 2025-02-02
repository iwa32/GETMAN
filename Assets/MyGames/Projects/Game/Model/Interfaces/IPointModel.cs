using UniRx;

namespace GameModel
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
        /// ポイントのセット
        /// </summary>
        void SetPoint(int point);
    }
}
