using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using UniRx;

namespace GameView
{
    public class GameOverView : MonoBehaviour
    {
        [SerializeField]
        [Header("リトライボタンを設定")]
        Button _retryButton;

        [SerializeField]
        [Header("タイトルボタンを設定")]
        Button _toTitleButton;

        public IObservable<Unit> ClickRetryButton()
        {
            return ObservableClickButton(_retryButton);
        }

        public IObservable<Unit> ClickToTitleButton()
        {
            return ObservableClickButton(_toTitleButton);
        }

        /// <summary>
        /// クリックイベントを通知します
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        IObservable<Unit> ObservableClickButton(Button button)
        {
            return button.OnClickAsObservable()
                .ThrottleFirst(TimeSpan.FromMilliseconds(1000));//二重送信防止
        }
    }
}