using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace GameModel
{
    public class ScoreModel : IScoreModel
    {
        IntReactiveProperty _score = new IntReactiveProperty();

        public IReadOnlyReactiveProperty<int> Score => _score;
        

        public void AddScore(int score)
        {
            _score.Value += score;
        }

        public void SetScore(int score)
        {
            _score.Value = score;
        }
    }
}
