using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Zenject;
using CountDownTimer;

namespace GameView
{
    public class GameStartView : MonoBehaviour
    {
        [SerializeField]
        [Header("カウント用のテキストを設定")]
        Text _countText;

        [SerializeField]
        [Header("ゲーム開始のアナウンス用テキストを設定")]
        Text _gameStartText;

        [SerializeField]
        [Header("ゲーム開始までのカウントの秒数")]
        int _gameStartCount = 3;

        BoolReactiveProperty _isGameStart = new BoolReactiveProperty();
        BoolReactiveProperty _isOpendGameStartText = new BoolReactiveProperty();
        IObservableCountDownTimer _gameStartCountDown;//ゲーム開始時のカウントダウン

        public IReadOnlyReactiveProperty<bool> IsGameStart => _isGameStart;

        [Inject]
        public void Construct(
            IObservableCountDownTimer countDownTimer
        )
        {
            _gameStartCountDown = countDownTimer;
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
                    CloseUIFor(_countText.gameObject);
                    OpenUIFor(_gameStartText.gameObject);
                    _isOpendGameStartText.Value = true;
                }
                );

            //ゲーム開始テキストを1秒後非表示にします
            _isOpendGameStartText
                .Where(isOpend => isOpend == true)
                .Delay(TimeSpan.FromSeconds(1))
                .Subscribe(_ =>
                {
                    CloseUIFor(_gameStartText.gameObject);
                    CloseUIFor(gameObject);
                    _isGameStart.Value = true;
                })
                .AddTo(this);
        }

        public void Initialize()
        {
            _isGameStart.Value = false;
            _isOpendGameStartText.Value = false;

            _gameStartCountDown.SetCountTime(_gameStartCount);
            _gameStartCountDown.Publish();
            OpenUIFor(gameObject);
            OpenUIFor(_countText.gameObject);
            CloseUIFor(_gameStartText.gameObject);
        }

        public void StartCount()
        {
            _gameStartCountDown.Connect();
        }

        /// <summary>
        /// UIを表示します
        /// </summary>
        /// <param name="target"></param>
        void OpenUIFor(GameObject target)
        {
            target?.SetActive(true);
        }

        /// <summary>
        /// UIを非表示にします
        /// </summary>
        /// <param name="target"></param>
        void CloseUIFor(GameObject target)
        {
            target?.SetActive(false);
        }
    }

}