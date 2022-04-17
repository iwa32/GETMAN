using UniRx;

namespace Pause
{
    public interface IPause
    {
        IReactiveProperty<bool> IsPause { get; }

        /// <summary>
        /// 一時停止を切り替えます
        /// </summary>
        /// <param name="isPause"></param>
        void ChangePause(bool isPause);

        /// <summary>
        /// 一時停止します
        /// </summary>
        void DoPause();

        /// <summary>
        /// 再開します
        /// </summary>
        void Resume();
    }
}