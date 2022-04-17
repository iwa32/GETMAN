using UniRx;

namespace GameModel
{
    /// <summary>
    /// ゲームの進行に関するモデル
    /// </summary>
    public interface IDirectionModel
    {
        IReadOnlyReactiveProperty<bool> IsGameStart { get; }
        IReadOnlyReactiveProperty<bool> IsGameOver { get; }
        IReadOnlyReactiveProperty<bool> IsGameClear { get; }
        IReadOnlyReactiveProperty<bool> IsGameContinue { get; }
        IReadOnlyReactiveProperty<bool> IsGamePause { get; }

        void SetIsGameStart(bool isGameStart);
        void SetIsGameOver(bool isGameOver);
        void SetIsGameClear(bool isGameClear);
        void SetIsGameContinue(bool isGameContinue);
        void SetIsGamePause(bool isGamePause);

        /// <summary>
        /// ゲームの状態をリセットします
        /// </summary>
        void ResetGame();

        /// <summary>
        /// ゲームができるか
        /// </summary>
        /// <returns></returns>
        bool CanGame();

        /// <summary>
        /// ゲームが終了したか
        /// </summary>
        /// <returns></returns>
        bool IsEndedGame();
    }
}