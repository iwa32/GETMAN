using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace GameModel
{
    public class DirectionTest
    {
        DirectionModel _directionModel;

        [SetUp]
        public void SetUp()
        {
            _directionModel = new DirectionModel();
        }

        [Test, Description("bool値が正しく設定される")]
        public void SetGameBoolTest([Values(true, false)] bool expected)
        {
            _directionModel.SetIsGameStart(expected);
            _directionModel.SetIsGameOver(expected);
            _directionModel.SetIsGameClear(expected);
            _directionModel.SetIsGameContinue(expected);

            Assert.That(_directionModel.IsGameStart.Value, Is.EqualTo(expected));
            Assert.That(_directionModel.IsGameOver.Value, Is.EqualTo(expected));
            Assert.That(_directionModel.IsGameClear.Value, Is.EqualTo(expected));
            Assert.That(_directionModel.IsGameContinue.Value, Is.EqualTo(expected));
        }

        [Test, Description("ゲームの状態がリセットできる")]
        public void ResetGameTest()
        {
            //フラグをtrueにセット
            _directionModel.SetIsGameStart(true);
            _directionModel.SetIsGameOver(true);
            _directionModel.SetIsGameClear(true);
            _directionModel.SetIsGameContinue(true);
            //リセット
            _directionModel.ResetGame();

            Assert.That(_directionModel.IsGameStart.Value, Is.False);
            Assert.That(_directionModel.IsGameOver.Value, Is.False);
            Assert.That(_directionModel.IsGameClear.Value, Is.False);
            Assert.That(_directionModel.IsGameContinue.Value, Is.False);
        }
    }
}
