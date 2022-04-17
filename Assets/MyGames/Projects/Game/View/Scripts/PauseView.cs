using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UIUtility;
using UniRx;
using UnityEngine.EventSystems;

namespace GameView
{
    public class PauseView : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField]
        [Header("停止アイコンを設定")]
        Sprite _pauseIcon;

        [SerializeField]
        [Header("再開アイコンを設定")]
        Sprite _resumeIcon;


        Button _pauseButton;
        Image _pauseButtonImage;
        BoolReactiveProperty _isPause = new BoolReactiveProperty();

        public IReactiveProperty<bool> IsPause => _isPause;

        void Start()
        {
            Initialize();
        }

        void Initialize()
        {
            SetPauseButton();
        }

        void SetPauseButton()
        {
            _pauseButton = GetComponent<Button>();
            _pauseButtonImage = _pauseButton.GetComponent<Image>();
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

        /// <summary>
        /// クリックでポーズフラグをオンにします
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerClick(PointerEventData eventData)
        {
            //ButtonをObsesrvable化するとゲーム再開時に購読できないため、EventSystemsで代用します
            _isPause.Value = !_isPause.Value;
        }
    }
}
