using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UniRx
{
    /// <summary>
    /// TMProはAsObservableがないため、自前で作成する
    /// </summary>
    public static class UnityUIComponentExtensions
    {
        public static IDisposable SubscribeToText(this IObservable<string> source, TextMeshProUGUI text)
        {
            return source.SubscribeWithState(text, (x, t) => t.text = x);
        }

        public static IDisposable SubscribeToText<T>(this IObservable<T> source, TextMeshProUGUI text)
        {
            return source.SubscribeWithState(text, (x, t) => t.text = x.ToString());
        }

        public static IObservable<string> OnValueChangeAsObservable(this TMP_InputField inputField)
        {
            return Observable.CreateWithState<string, TMP_InputField>(inputField, (i, observer) =>
            {
                observer.OnNext(i.text);
                return i.OnValueChangeAsObservable().Subscribe(observer);
            });
        }

        public static IObservable<string> OnValueChangedAsObservable(this TMP_InputField inputField)
        {
            return Observable.CreateWithState<string, TMP_InputField>(inputField, (i, observer) =>
            {
                observer.OnNext(i.text);
                return i.onValueChanged.AsObservable().Subscribe(observer);
            });
        }

        public static IObservable<string> OnEndEditAsObservable(this TMP_InputField inputField)
        {
            return Observable.CreateWithState<string, TMP_InputField>(inputField, (i, observer) =>
            {
                observer.OnNext(i.text);
                return i.onEndEdit.AsObservable().Subscribe(observer);
            });
        }
    }
}