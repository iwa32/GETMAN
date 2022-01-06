using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using PlayerModel;

public class ScoreTest
{
    ScoreModel _scoreModel;
    int initialScore = 0;

    [SetUp]
    public void SetUp()
    {
        _scoreModel = new ScoreModel(initialScore);
    }

    [Test, Description("スコアが加算されているか")]
    [TestCase(1, 1)]
    [TestCase(0, 0)]
    [TestCase(-1, -1)]
    public void AddScoreTest(int input, int expected)
    {
        _scoreModel.AddScore(input);

        Assert.That(_scoreModel.Score.Value, Is.EqualTo(expected));
    }

    [Test, Description("スコアがリセットされいるか")]
    public void ResetScoreTest([Values(1, -1, 2)] int input)
    {
        _scoreModel.AddScore(input);
        _scoreModel.ResetScore();

        Assert.That(_scoreModel.Score.Value, Is.EqualTo(initialScore));
    }
}
