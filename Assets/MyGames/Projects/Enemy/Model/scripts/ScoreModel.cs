using UniRx;

namespace EnemyModel
{
    public class ScoreModel : IScoreModel
    {
        IntReactiveProperty _score = new IntReactiveProperty();

        public IReadOnlyReactiveProperty<int> Score => _score;

        public void SetScore(int score)
        {
            _score.Value = score;
        }
    }
}
