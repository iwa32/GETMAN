using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace GameModel
{
    public class GameTest
    {
        GameModel _gameModel;

        [SetUp]
        public void SetUp()
        {
            _gameModel = new GameModel();
        }

        [Test, Description("bool値が正しく設定される")]
        public void SetGameBoolTest([Values(true, false)] bool expected)
        {
            _gameModel.SetIsGameStart(expected);
            _gameModel.SetIsGameOver(expected);
            _gameModel.SetIsGameClear(expected);
            _gameModel.SetIsGameContinue(expected);

            Assert.That(_gameModel.IsGameStart.Value, Is.EqualTo(expected));
            Assert.That(_gameModel.IsGameOver.Value, Is.EqualTo(expected));
            Assert.That(_gameModel.IsGameClear.Value, Is.EqualTo(expected));
            Assert.That(_gameModel.IsGameContinue.Value, Is.EqualTo(expected));
        }

        [Test, Description("ゲームの状態がリセットできる")]
        public void ResetGameTest()
        {
            //フラグをtrueにセット
            _gameModel.SetIsGameStart(true);
            _gameModel.SetIsGameOver(true);
            _gameModel.SetIsGameClear(true);
            _gameModel.SetIsGameContinue(true);
            //リセット
            _gameModel.ResetGame();

            Assert.That(_gameModel.IsGameStart.Value, Is.False);
            Assert.That(_gameModel.IsGameOver.Value, Is.False);
            Assert.That(_gameModel.IsGameClear.Value, Is.False);
            Assert.That(_gameModel.IsGameContinue.Value, Is.False);
        }
    }
}
