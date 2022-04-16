using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using SoundModel;
using Zenject;

namespace SoundManager
{
    public class SoundManager : MonoBehaviour,
        ISoundManager
    {
        [SerializeField]
        [Header("Bgmの音量")]
        float _initBgmVolume = 0.6f;

        [SerializeField]
        [Header("SEの音量")]
        float _initSEVolume = 0.7f;

        [SerializeField]
        [Header("Bgmのスクリプタブルオブジェクトを設定")]
        BgmDataList _bgmDataList;

        [SerializeField]
        [Header("SEのスクリプタブルオブジェクトを設定")]
        SEDataList _seDataList;

        #region//フィールド
        AudioSource _bgmSource;
        AudioSource _seSource;
        ISoundModel _soundModel;
        #endregion

        void Awake()
        {
            InitializeBgm();
            InitializeSE();
        }

        void Start()
        {
            Bind();
        }

        [Inject]
        public void Construct(ISoundModel soundModel)
        {
            _soundModel = soundModel;
        }

        /// <summary>
        /// Bgmの設定をします
        /// </summary>
        void InitializeBgm()
        {
            _bgmSource = gameObject.AddComponent<AudioSource>();
            _soundModel.SetBgmVolume(_initBgmVolume);
            _bgmSource.playOnAwake = false;
            _bgmSource.loop = true;
        }

        /// <summary>
        /// SEの設定をします
        /// </summary>
        void InitializeSE()
        {
            _seSource = gameObject.AddComponent<AudioSource>();
            _soundModel.SetSEVolume(_initSEVolume);
            _seSource.playOnAwake = false;
        }

        void Bind()
        {
            //音量
            _soundModel.BgmVolume
                .Subscribe(value => _bgmSource.volume = value)
                .AddTo(this);

            _soundModel.SEVolume
                .Subscribe(value => _seSource.volume = value)
                .AddTo(this);
            //ミュート
            _soundModel.BgmIsMute
                .Subscribe(isMute => _bgmSource.mute = isMute)
                .AddTo(this);

            _soundModel.SEIsMute
                .Subscribe(isMute => _seSource.mute = isMute)
                .AddTo(this);
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

            _seSource?.PlayOneShot(seClip, _initSEVolume);
        }
    }
}
