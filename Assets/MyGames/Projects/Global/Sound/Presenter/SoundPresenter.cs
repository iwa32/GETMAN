using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Zenject;
using SoundView;
using SoundModel;

namespace SoundPresenter
{
    public class SoundPresenter : MonoBehaviour
    {
        [SerializeField]
        SoundView.SoundView _soundView;

        #region//フィールド
        ISoundModel _soundModel;
        #endregion

        [Inject]
        public void Construct(
            ISoundModel soundModel
        )
        {
            _soundModel = soundModel;
        }

        private void Start()
        {
            _soundView.Initialize();
            Bind();
        }

        void Bind()
        {
            //---音声オプションを開く---
            _soundView.ButtonToOpenCanvas
                .Subscribe(_ => _soundView.OpenSoundOption())
                .AddTo(this);
            //---音声オプションを閉じる---
            _soundView.ButtonToCloseCanvas
                .Subscribe(_ => _soundView.CloseSoundOption())
                .AddTo(this);

            _soundView.OverlayToCloseCanvas
                .Subscribe(_ => _soundView.CloseSoundOption())
                .AddTo(this);

            //---ミュートボタン---
            //view to model
            //BGM
            _soundView.BgmMuteButton
                .Subscribe(_ => _soundModel.SetBgmIsMute(!_soundModel.BgmIsMute.Value))
                .AddTo(this);
            //SE
            _soundView.SEMuteButton
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
            _soundView.BgmSlider
                .Subscribe(value => _soundModel.SetBgmVolume(value))
                .AddTo(this);

            //SE
            _soundView.SESlider
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
