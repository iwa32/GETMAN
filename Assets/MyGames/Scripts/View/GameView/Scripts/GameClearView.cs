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
        [Header("次のステージへのボタンを設定")]
        Button _nextStageButton;

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

        public IObservable<Unit> ClickNextStageButton()
        {
            return _observableClickButton.CreateObservableClickButton(_nextStageButton);
        }

        public IObservable<Unit> ClickToTitleButton()
        {
            return _observableClickButton.CreateObservableClickButton(_toTitleButton);
        }
    }
}