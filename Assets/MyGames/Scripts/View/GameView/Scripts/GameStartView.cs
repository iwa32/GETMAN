using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;
using Zenject;
using CountDownTimer;
using UIUtility;

namespace GameView
{
    public class GameStartView : MonoBehaviour
    {
        [SerializeField]
        [Header("カウント用のテキストを設定")]
        TextMeshProUGUI _countText;

        [SerializeField]
        [Header("ゲーム開始のアナウンス用テキストを設定")]
        TextMeshProUGUI _gameStartText;

        [SerializeField]
        [Header("ゲーム開始までのカウントの秒数")]
        int _gameStartCount = 3;

        BoolReactiveProperty _isGameStart = new BoolReactiveProperty();
        BoolReactiveProperty _isOpendGameStartText = new BoolReactiveProperty();
        IObservableCountDownTimer _gameStartCountDown;//ゲーム開始時のカウントダウン
        IToggleableUI _toggleableUI;

        public IReadOnlyReactiveProperty<bool> IsGameStart => _isGameStart;

        [Inject]
        public void Construct(
            IObservableCountDownTimer countDownTimer,
            IToggleableUI toggleableUI
        )
        {
            _gameStartCountDown = countDownTimer;
            _toggleableUI = toggleableUI;
        }

        void Start()
        {
            ////カウントダウンをし、終了後Game開始のUIを表示
            _gameStartCountDown.CountDownObservable
                .Subscribe(time =>
                {
                    _countText.text = time.ToString();
                },
                () =>
                {
                    _toggleableUI.CloseUIFor(_countText.gameObject);
                    _toggleableUI.OpenUIFor(_gameStartText.gameObject);
                    _isOpendGameStartText.Value = true;
                }
                );

            //ゲーム開始テキストを1秒後非表示にします
            _isOpendGameStartText
                .Where(isOpend => isOpend == true)
                .Delay(TimeSpan.FromSeconds(1))
                .Subscribe(_ =>
                {
                    _toggleableUI.CloseUIFor(_gameStartText.gameObject);
                    _toggleableUI.CloseUIFor(gameObject);
                    _isGameStart.Value = true;
                })
                .AddTo(this);
        }

        public void Initialize()
        {
            _isGameStart.Value = false;
            _isOpendGameStartText.Value = false;

            //カウントダウンの設定、呼び出しの待機
            _gameStartCountDown.SetCountTime(_gameStartCount);
            _gameStartCountDown.Publish();
            //uiの初期表示状態
            _toggleableUI.OpenUIFor(gameObject);
            _toggleableUI.OpenUIFor(_countText.gameObject);
            _toggleableUI.CloseUIFor(_gameStartText.gameObject);
        }

        public void StartCount()
        {
            _gameStartCountDown.Connect();
        }
    }
}