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
        [Header("リスタートボタンを設定")]
        Button _restartButton;

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

        public IObservable<Unit> ClickRestartButton()
        {
            return _observableClickButton.CreateObservableClickButton(_restartButton);
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