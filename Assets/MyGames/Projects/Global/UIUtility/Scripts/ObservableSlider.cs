using System;
using UnityEngine.UI;
using UniRx;

namespace UIUtility
{
    public class ObservableSlider: IObservableSlider
    {
        /// <summary>
        /// 値の変化時に発火するスライダーイベントを作成します
        /// </summary>
        /// <param name="slider"></param>
        /// <returns></returns>
        public IObservable<float> CreateObservableSliderOnValueChanged(Slider slider)
        {
            return slider.OnValueChangedAsObservable()
                .Skip(1);//初回起動時の呼び出しは無視します
        }
    }
}
