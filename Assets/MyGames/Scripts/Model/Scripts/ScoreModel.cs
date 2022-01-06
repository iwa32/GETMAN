using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace PlayerModel
{
    public class ScoreModel : IScoreModel
    {
        public IReadOnlyReactiveProperty<int> Score => _score;

        IntReactiveProperty _score = new IntReactiveProperty();
        int initialScore;

        internal ScoreModel(int score)
        {
            _score.Value = score;
            initialScore = score;
        }

        public void AddScore(int score)
        {
            _score.Value += score;
        }

        public void ResetScore()
        {
            _score.Value = initialScore;
        }
    }
}
