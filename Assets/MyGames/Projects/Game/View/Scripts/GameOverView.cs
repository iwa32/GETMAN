using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Zenject;
using UIUtility;

namespace GameView
{
    public class GameOverView : MonoBehaviour
    {
        [SerializeField]
        [Header("コンティニューボタンを設定")]
        Button _continueButton;

        [SerializeField]
        [Header("セーブボタンを設定")]
        Button _saveButton;

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

        public IObservable<Unit> ClickSaveButton()
        {
            return _observableClickButton.CreateObservableClickButton(_saveButton);
        }

        public IObservable<Unit> ClickToTitleButton()
        {
            return _observableClickButton.CreateObservableClickButton(_toTitleButton);
        }
    }
}