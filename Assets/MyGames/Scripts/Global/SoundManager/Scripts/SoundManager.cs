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

        [SerializeField]
        [Header("Bgmのスクリプタブルオブジェクトを設定")]
        BgmDataList _bgmDataList;

        [SerializeField]
        [Header("SEのスクリプタブルオブジェクトを設定")]
        SEDataList _seDataList;

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

        /// <summary>
        /// Bgmを再生します
        /// </summary>
        /// <param name="bgmType"></param>
        public void PlayBgm(BgmType bgmType)
        {
            AudioClip bgmClip = _bgmDataList.FindBgmClipByType(bgmType);
            if (bgmClip == null) return;

            _bgmSource.Stop();//現在流れているbgmを停止
            _bgmSource.clip = bgmClip;
            _bgmSource.Play();
        }

        /// <summary>
        /// bgmを停止します
        /// </summary>
        public void StopBgm()
        {
            _bgmSource.Stop();
        }

        /// <summary>
        /// SEを再生します
        /// </summary>
        /// <param name="seType"></param>
        public void PlaySE(SEType seType)
        {
            AudioClip seClip = _seDataList.FindSEDataByType(seType);
            if (seClip == null) return;

            _seSource.PlayOneShot(seClip, _seVolume);
        }
    }
}
