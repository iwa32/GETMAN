using System.Collections;
using System.Collections.Generic;
using UniRx;

namespace GameModel
{
    public class GameModel : IGameModel
    {
        BoolReactiveProperty _isGameStart = new BoolReactiveProperty();
        BoolReactiveProperty _isGameOver = new BoolReactiveProperty();
        BoolReactiveProperty _isGameClear = new BoolReactiveProperty();
        BoolReactiveProperty _isGameContinue = new BoolReactiveProperty();

        public IReadOnlyReactiveProperty<bool> IsGameStart => _isGameStart;
        public IReadOnlyReactiveProperty<bool> IsGameOver => _isGameOver;
        public IReadOnlyReactiveProperty<bool> IsGameClear => _isGameClear;
        public IReadOnlyReactiveProperty<bool> IsGameContinue => _isGameContinue;

        public void SetIsGameStart(bool isGameStart)
        {
            _isGameStart.Value = isGameStart;
        }

        public void SetIsGameOver(bool isGameOver)
        {
            _isGameOver.Value = isGameOver;
        }

        public void SetIsGameClear(bool isGameClear)
        {
            _isGameClear.Value = isGameClear;
        }

        public void SetIsGameContinue(bool isGameContinue)
        {
            _isGameContinue.Value = isGameContinue;
        }

        public void ResetGame()
        {
            _isGameStart.Value = false;
            _isGameOver.Value = false;
            _isGameClear.Value = false;
            _isGameContinue.Value = false;
        }

        public bool CanGame()
        {
            return (_isGameStart.Value && _isGameOver.Value == false);
        }
    }
}
