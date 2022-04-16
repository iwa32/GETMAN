using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace SoundModel
{
    public class SoundModel: ISoundModel
    {
        ReactiveProperty<float> _bgmVolume = new ReactiveProperty<float>();
        ReactiveProperty<float> _seVolume = new ReactiveProperty<float>();
        BoolReactiveProperty _bgmIsMute = new BoolReactiveProperty();
        BoolReactiveProperty _seIsMute = new BoolReactiveProperty();

        public IReadOnlyReactiveProperty<float> BgmVolume => _bgmVolume;
        public IReadOnlyReactiveProperty<float> SEVolume => _seVolume;
        public IReadOnlyReactiveProperty<bool> BgmIsMute => _bgmIsMute;
        public IReadOnlyReactiveProperty<bool> SEIsMute => _seIsMute;

        public void SetBgmVolume(float volume)
        {
            _bgmVolume.Value = volume;
        }

        public void SetSEVolume(float volume)
        {
            _seVolume.Value = volume;
        }

        public void SetBgmIsMute(bool isMute)
        {
            _bgmIsMute.Value = isMute;
        }

        public void SetSEIsMute(bool isMute)
        {
            _seIsMute.Value = isMute;
        }
    } 
}