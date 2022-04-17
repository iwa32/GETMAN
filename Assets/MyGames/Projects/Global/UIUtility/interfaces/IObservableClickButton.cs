using System;
using UniRx;
using UnityEngine.UI;

namespace UIUtility
{
    public interface IObservableClickButton
    {
        /// <summary>
        /// クリックイベントを通知します
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        IObservable<Unit> CreateObservableClickButton(Button button);

        /// <summary>
        /// ポーズ中はボタンのストリームを再購読できないため、破棄し再生成します
        /// </summary>
        /// <param name="muteButton"></param>
        /// <param name="action"></param>
        void RepeatObserveButtonForPause(IObservable<Unit> button, Action action);
    }
}