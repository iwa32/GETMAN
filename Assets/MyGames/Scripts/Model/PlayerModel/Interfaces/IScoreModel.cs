using UniRx;

namespace PlayerModel
{
    public interface IScoreModel
    {
        IReadOnlyReactiveProperty<int> Score { get; }

        /// <summary>
        /// スコアの増加
        /// </summary>
        /// <param name="score"></param>
        void AddScore(int score);

        /// <summary>
        /// スコアのセット
        /// </summary>
        void SetScore(int score);
    }
}