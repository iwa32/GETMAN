using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;

namespace Fade
{
    public class Fade : MonoBehaviour,
        IFade
    {
        [SerializeField]
        [Header("フェードする時間を指定する")]
        float _fadeDuration;

        [SerializeField]
        [Header("初回起動時にフェードインが完了しているか")]
        bool _isFirstFadeInComp;

        CanvasGroup _canvasGroup;

        void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            Initialize();
        }

        void Initialize()
        {
            if (_canvasGroup == null) return;
            //初期化時は非表示として扱う
            _canvasGroup.alpha = 0;
            _canvasGroup.blocksRaycasts = false;
        }

        public async UniTask StartFadeOut()
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.blocksRaycasts = true;
            await DoFade(1);
        }

        public async UniTask StartFadeIn()
        {
            if (CheckFirstFadeInComp()) return;

            _canvasGroup.alpha = 1;
            _canvasGroup.blocksRaycasts = false;

            await DoFade(0);
        }

        public async UniTask FadeInBeforeAction(Action action)
        {
            await StartFadeIn();
            action();
        }

        public async UniTask FadeOutBeforeAction(Action action)
        {
            await StartFadeOut();
            action();
        }

        /// <summary>
        /// フェードの処理を行います
        /// </summary>
        /// <param name="endValue"></param>
        /// <returns></returns>
        async UniTask DoFade(int endValue)
        {
            await _canvasGroup
                .DOFade(endValue, _fadeDuration)
                .AsyncWaitForCompletion();
        }

        /// <summary>
        /// 初回起動時にフェードインを完了させているか確認します
        /// </summary>
        /// <returns></returns>
        bool CheckFirstFadeInComp()
        {
            if (_isFirstFadeInComp)
            {
                _isFirstFadeInComp = false;//次回以降はフェードインさせる
                return true;
            }
            return false;
        }
    }
}