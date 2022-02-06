using UniRx;
using System;
using UnityEngine.UI;

namespace UIUtility
{
    public class ObservableClickButton : IObservableClickButton
    {
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
    }
}
