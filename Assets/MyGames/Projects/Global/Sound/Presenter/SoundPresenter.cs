using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Zenject;
using UIUtility;
using SoundView;
using SoundModel;

namespace SoundPresenter
{
    public class SoundPresenter : MonoBehaviour
    {
        [SerializeField]
        [Header("開くボタン(サウンドオプション)")]
        Button _buttonToOpenCanvas;

        [SerializeField]
        [Header("閉じるボタン(サウンドオプション)")]
        Button _buttonToCloseCanvas;

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

        [SerializeField]
        SoundView.SoundView _soundView;

        #region//フィールド
        IObservableClickButton _observableClickButton;
        IObservableSlider _observableSlider;
        ISoundModel _soundModel;
        #endregion

        [Inject]
        public void Construct(
            IObservableClickButton observableClickButton,
            IObservableSlider observableSlider,
            ISoundModel soundModel
        )
        {
            _observableClickButton = observableClickButton;
            _observableSlider = observableSlider;
            _soundModel = soundModel;
        }

        private void Start()
        {
            Bind();
        }

        void Bind()
        {
            //---音声オプションを開く---
            _observableClickButton.CreateObservableClickButton(_buttonToOpenCanvas)
                .Subscribe(_ => _soundView.OpenSoundOption())
                .AddTo(this);

            //---音声オプションを閉じる---
            _observableClickButton.CreateObservableClickButton(_buttonToCloseCanvas)
                .Subscribe(_ => _soundView.CloseSoundOption())
                .AddTo(this);

            _observableClickButton.CreateObservableClickButton(_overlayToCloseCanvas)
                .Subscribe(_ => _soundView.CloseSoundOption())
                .AddTo(this);

            //---ミュートボタン---
            //view to model
            //BGM
            _observableClickButton.CreateObservableClickButton(_bgmMuteButton)
                .Subscribe(_ => _soundModel.SetBgmIsMute(!_soundModel.BgmIsMute.Value))
                .AddTo(this);
            //SE
            _observableClickButton.CreateObservableClickButton(_seMuteButton)
                .Subscribe(_ => _soundModel.SetSEIsMute(!_soundModel.SEIsMute.Value))
                .AddTo(this);

            //model to view
            //BGM
            _soundModel.BgmIsMute
                .Subscribe(isMute => _soundView.ToggleBgmMuteIcon(isMute))
                .AddTo(this);
            //SE
            _soundModel.SEIsMute
                .Subscribe(isMute => _soundView.ToggleSEMuteIcon(isMute))
                .AddTo(this);


            //---音声スライダー---
            //view to model
            //BGM
            _observableSlider.CreateObservableSliderOnValueChanged(_bgmSlider)
                .Subscribe(value => _soundModel.SetBgmVolume(value))
                .AddTo(this);

            //SE
            _observableSlider.CreateObservableSliderOnValueChanged(_seSlider)
                .Subscribe(value => _soundModel.SetSEVolume(value))
                .AddTo(this);

            //model to view
            //BGM
            _soundModel.BgmVolume
                .Subscribe(value => _bgmSlider.value = value)
                .AddTo(this);
            //SE
            _soundModel.SEVolume
                .Subscribe(value => _seSlider.value = value)
                .AddTo(this);
        }
    }
}
