using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace EnemyModel
{
    public class ScoreTest
    {
        ScoreModel _scoreModel;
        int _initialScore = 0;

        [SetUp]
        public void SetUp()
        {
            _scoreModel = new ScoreModel();
            _scoreModel.SetScore(_initialScore);
        }

        [Test, Description("スコアが正しくセットされているか")]
        public void SetScoreTest([Values(1, -1, 2)] int expected)
        {
            _scoreModel.SetScore(expected);

            Assert.That(_scoreModel.Score.Value, Is.EqualTo(expected));
        }
    }
}
