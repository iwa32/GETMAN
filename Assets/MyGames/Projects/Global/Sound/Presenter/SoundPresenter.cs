using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Zenject;
using SoundView;
using SoundModel;
using Pause;
using UIUtility;

namespace SoundPresenter
{
    public class SoundPresenter : MonoBehaviour
    {
        [SerializeField]
        SoundView.SoundView _soundView;

        #region//フィールド
        ISoundModel _soundModel;
        IObservableClickButton _observableClickButton;
        IPause _pause;
        #endregion

        [Inject]
        public void Construct(
            ISoundModel soundModel,
            IObservableClickButton observableClickButton,
            IPause pause
        )
        {
            _soundModel = soundModel;
            _observableClickButton = observableClickButton;
            _pause = pause;
        }

        private void Start()
        {
            _soundView.Initialize();
            Bind();
        }

        void Bind()
        {
            //---音声オプションを開く---
            _soundView.ButtonToOpenCanvasAsObservable
                .Subscribe(_ =>
                {
                    _soundView.OpenSoundOption();
                    _pause.DoPause();
                })
                .AddTo(this);

            //---音声オプションを閉じる---
            _soundView.OverlayToCloseCanvasAsObservable
                .Subscribe(_ =>
                {
                    _soundView.CloseSoundOption();
                    _pause.Resume();
                })
                .AddTo(this);

            //---ミュートボタン---
            //view to model
            //ポーズ中はボタンのストリームを再購読できないため、破棄し再生成します
            //BGM
            Action bgmMuteAction = () => _soundModel.SetBgmIsMute(!_soundModel.BgmIsMute.Value);
            _observableClickButton
                .RepeatObserveButtonForPause(_soundView.BgmMuteButtonAsObservable, bgmMuteAction);

            //SE
            Action seMuteAction = () => _soundModel.SetSEIsMute(!_soundModel.SEIsMute.Value);
            _observableClickButton.RepeatObserveButtonForPause(_soundView.SEMuteButtonAsObservable, seMuteAction);

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
            _soundView.BgmSliderAsObservable
                .Subscribe(value => _soundModel.SetBgmVolume(value))
                .AddTo(this);

            //SE
            _soundView.SESliderAsObservable
                .Subscribe(value => _soundModel.SetSEVolume(value))
                .AddTo(this);

            //model to view
            //BGM
            _soundModel.BgmVolume
                .Subscribe(value => _soundView.SetBgmSliderValue(value))
                .AddTo(this);
            //SE
            _soundModel.SEVolume
                .Subscribe(value => _soundView.SetSESliderValue(value))
                .AddTo(this);
        }
    }
}
