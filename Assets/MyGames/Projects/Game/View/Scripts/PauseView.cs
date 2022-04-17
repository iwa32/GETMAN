using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UIUtility;
using UniRx;

namespace GameView
{
    public class PauseView : MonoBehaviour
    {
        [SerializeField]
        [Header("停止アイコンを設定")]
        Sprite _pauseIcon;

        [SerializeField]
        [Header("再開アイコンを設定")]
        Sprite _resumeIcon;


        Button _pauseButton;
        Image _pauseButtonImage;

        #region//フィールド
        IObservableClickButton _observableClickButton;
        IObservable<Unit> _pauseButtonAsObservable;
        #endregion

        public IObservable<Unit> PauseButtonAsObservable => _pauseButtonAsObservable;

        [Inject]
        public void Construct(
            IObservableClickButton observableClickButton
        )
        {
            _observableClickButton = observableClickButton;
        }

        void Start()
        {
            Initialize();
        }

        void Initialize()
        {
            SetPauseButton();
            CreateObservable();
        }

        void SetPauseButton()
        {
            _pauseButton = GetComponent<Button>();
            _pauseButtonImage = _pauseButton.GetComponent<Image>();
        }

        void CreateObservable()
        {
            _pauseButtonAsObservable
                = _observableClickButton.CreateObservableClickButton(_pauseButton);
        }

        // <summary>
        /// ポーズアイコンを切り替えます
        /// </summary>
        /// <param name="isMute"></param>
        public void TogglePauseIcon(bool isPause)
        {
            Sprite pauseIcon = GetPauseIcon(isPause);
            _pauseButtonImage.sprite = pauseIcon;
        }

        /// <summary>
        /// ポーズアイコンを取得します
        /// </summary>
        /// <param name="isMute"></param>
        /// <returns></returns>
        Sprite GetPauseIcon(bool isPause)
        {
            if (isPause) return _resumeIcon;
            return _pauseIcon;
        }
    }
}
