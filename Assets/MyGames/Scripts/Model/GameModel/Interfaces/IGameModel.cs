using UniRx;

namespace GameModel
{
    public interface IGameModel
    {
        IReadOnlyReactiveProperty<bool> IsGameStart { get; }
        IReadOnlyReactiveProperty<bool> IsGameOver { get; }
        IReadOnlyReactiveProperty<bool> IsGameClear { get; }
        IReadOnlyReactiveProperty<bool> IsGameContinue { get; }

        void SetIsGameStart(bool isGameStart);
        void SetIsGameOver(bool isGameOver);
        void SetIsGameClear(bool isGameClear);
        void SetIsGameContinue(bool isGameContinue);

        /// <summary>
        /// ゲームの状態をリセットします
        /// </summary>
        void ResetGame();
    }
}