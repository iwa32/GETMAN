using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoundManager
{
    public class SoundManager : MonoBehaviour,
        ISoundManager
    {
        [SerializeField]
        [Header("Bgmの音量")]
        float _bgmVolume = 1.0f;

        [SerializeField]
        [Header("SEの音量")]
        float _seVolume = 0.3f;

        AudioSource _bgmSource;
        AudioSource _seSource;

        void Awake()
        {
            InitializeBgm();
            InitializeSE();
        }

        /// <summary>
        /// Bgmの設定をします
        /// </summary>
        void InitializeBgm()
        {
            _bgmSource = gameObject.AddComponent<AudioSource>();
            _bgmSource.volume = _bgmVolume;
            _bgmSource.playOnAwake = false;
            _bgmSource.loop = true;
        }

        /// <summary>
        /// SEの設定をします
        /// </summary>
        void InitializeSE()
        {
            _seSource = gameObject.AddComponent<AudioSource>();
            _seSource.volume = _seVolume;
            _seSource.playOnAwake = false;
        }

        public void PlayBgm(BgmType bgmType)
        {

        }

        public void PlayerSE(SEType seType )
        {

        }
    }
}
