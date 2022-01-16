using UniRx;

namespace EnemyModel
{
    public interface IScoreModel
    {
        IReadOnlyReactiveProperty<int> Score { get; }

        void SetScore(int score);
    }
}
