using System;
using UnityEngine.UI;
using UniRx;

namespace UIUtility
{
    public interface IObservableSlider
    {
        /// <summary>
        /// 値の変化時に発火するスライダーイベントを作成します
        /// </summary>
        /// <param name="slider"></param>
        /// <returns></returns>
        IObservable<float> CreateObservableSliderOnValueChanged(Slider slider);
    }
}