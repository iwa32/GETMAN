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
    }
}