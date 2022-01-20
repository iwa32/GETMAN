using System;
using System.Collections;
using System.Collections.Generic;
using UIUtility;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GameView
{
    public class GameClearView : MonoBehaviour
    {
        [SerializeField]
        [Header("コンティニューボタンを設定")]
        Button _continueButton;

        [SerializeField]
        [Header("タイトルボタンを設定")]
        Button _toTitleButton;

        IObservableClickButton _observableClickButton;

        [Inject]
        public void Construct(
            IObservableClickButton observableClickButton
        )
        {
            _observableClickButton = observableClickButton;
        }

        public IObservable<Unit> ClickContinueButton()
        {
            return _observableClickButton.CreateObservableClickButton(_continueButton);
        }

        public IObservable<Unit> ClickToTitleButton()
        {
            return _observableClickButton.CreateObservableClickButton(_toTitleButton);
        }
    }
}