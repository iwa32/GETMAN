using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UIUtility;
using UniRx;


namespace SoundView
{
    public class SoundView : MonoBehaviour
    {
        [SerializeField]
        [Header("音声オン時のアイコンを設定")]
        Sprite _volumeOnSprite;

        [SerializeField]
        [Header("音声オフ時のアイコンを設定")]
        Sprite _volumeOffSprite;

        [SerializeField]
        [Header("サウンドオプションのキャンバスを設定します")]
        Canvas _soundOptionCanvas;

        [SerializeField]
        [Header("開くボタン(サウンドオプション)")]
        Button _buttonToOpenCanvas;

        [SerializeField]
        [Header("閉じるためのオーバーレイを設定します(サウンドオプション)")]
        Button _overlayToCloseCanvas;

        [SerializeField]
        [Header("Bgmミュート用ボタンを設定")]
        Button _bgmMuteButton;

        [SerializeField]
        [Header("SEミュート用ボタンを設定")]
        Button _seMuteButton;

        [SerializeField]
        [Header("BGM調節用のスライダーを設定します")]
        Slider _bgmSlider;

        [SerializeField]
        [Header("SE調節用のスライダーを設定します")]
        Slider _seSlider;

        #region//フィールド
        Image _bgmMuteIcon;
        Image _seMuteIcon;
        IObservableClickButton _observableClickButton;
        IObservableSlider _observableSlider;
        IToggleableUI _toggleableUI;
        #endregion

        //Observable化
        //ボタン
        public IObservable<Unit> ButtonToOpenCanvas;
        public IObservable<Unit> OverlayToCloseCanvas;
        public IObservable<Unit> BgmMuteButton;
        public IObservable<Unit> SEMuteButton;
        //スライダー
        public IObservable<float> BgmSlider;
        public IObservable<float> SESlider;

        [Inject]
        public void Construct(
            IObservableClickButton observableClickButton,
            IObservableSlider observableSlider,
            IToggleableUI toggleableUI
        )
        {
            _observableClickButton = observableClickButton;
            _observableSlider = observableSlider;
            _toggleableUI = toggleableUI;
        }

        public void Initialize()
        {
            CreateObservable();
            SetMuteIconImage();
        }

        /// <summary>
        /// 各イベントをObservableにします
        /// </summary>
        void CreateObservable()
        {
            ButtonToOpenCanvas = _observableClickButton.CreateObservableClickButton(_buttonToOpenCanvas);
            OverlayToCloseCanvas = _observableClickButton.CreateObservableClickButton(_overlayToCloseCanvas);
            BgmMuteButton = _observableClickButton.CreateObservableClickButton(_bgmMuteButton);
            SEMuteButton = _observableClickButton.CreateObservableClickButton(_seMuteButton);
            BgmSlider = _observableSlider.CreateObservableSliderOnValueChanged(_bgmSlider);
            SESlider = _observableSlider.CreateObservableSliderOnValueChanged(_seSlider);
        }

        void SetMuteIconImage()
        {
            _bgmMuteIcon = _bgmMuteButton.GetComponent<Image>();
            _seMuteIcon = _seMuteButton.GetComponent<Image>();
        }
        
        public void OpenSoundOption()
        {
            _toggleableUI.OpenUIFor(_soundOptionCanvas?.gameObject);
        }

        public void CloseSoundOption()
        {
            _toggleableUI.CloseUIFor(_soundOptionCanvas?.gameObject);
        }

        public void SetBgmSliderValue(float value)
        {
            _bgmSlider.value = value;
        }

        public void SetSESliderValue(float value)
        {
            _seSlider.value = value;
        }

        /// <summary>
        /// bgmのミュートアイコンを切り替えます
        /// </summary>
        /// <param name="isMute"></param>
        public void ToggleBgmMuteIcon(bool isMute)
        {
            ToggleMuteIcon(_bgmMuteIcon, isMute);
        }

        /// <summary>
        /// SEのミュートアイコンを切り替えます
        /// </summary>
        /// <param name="isMute"></param>
        public void ToggleSEMuteIcon(bool isMute)
        {
            ToggleMuteIcon(_seMuteIcon, isMute);
        }

        void ToggleMuteIcon(Image targetImage, bool isMute)
        {
            Sprite volumeIcon = GetVolumeIcon(isMute);
            SetVolumeIcon(targetImage, volumeIcon);
        }

        /// <summary>
        /// 音声アイコンを取得します
        /// </summary>
        /// <param name="isMute"></param>
        /// <returns></returns>
        Sprite GetVolumeIcon(bool isMute)
        {
            if (isMute) return _volumeOffSprite;
            return _volumeOnSprite;
        }

        void SetVolumeIcon(Image targetImage, Sprite setSprite)
        {
            if (targetImage.sprite == setSprite) return;
            targetImage.sprite = setSprite;
        }
    }
}
