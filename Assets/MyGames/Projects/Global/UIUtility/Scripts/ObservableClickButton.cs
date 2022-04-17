using UniRx;
using System;
using UnityEngine.UI;

namespace UIUtility
{
    public class ObservableClickButton : IObservableClickButton, IDisposable
    {
        IDisposable _disposable;

        /// <summary>
        /// クリックイベントを通知します
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        public IObservable<Unit> CreateObservableClickButton(Button button)
        {
            return button.OnClickAsObservable()
                .ThrottleFirst(TimeSpan.FromMilliseconds(1000));//二重送信防止
        }

        /// <summary>
        /// ポーズ中はボタンのストリームを再購読できないため、破棄し再生成します
        /// </summary>
        /// <param name="muteButton"></param>
        /// <param name="action"></param>
        public void RepeatObserveButtonForPause(IObservable<Unit> button, Action action)
        {
            _disposable = button
                .First()
                .Subscribe(
                _ => action(),
                () => RepeatObserveButtonForPause(button, action)
                );
        }

        public void Dispose()
        {
            //ストリームの購読を止めます
            _disposable.Dispose();
        }
    }
}
