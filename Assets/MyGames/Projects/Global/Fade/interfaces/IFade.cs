using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Fade
{
    public interface IFade
    {
        /// <summary>
        /// フェードアウトを開始します
        /// </summary>
        /// <returns></returns>
        UniTask StartFadeOut();

        /// <summary>
        /// フェードインを開始します
        /// </summary>
        /// <returns></returns>
        UniTask StartFadeIn();

        /// <summary>
        /// アクション前にフェードインを実行します
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        UniTask FadeInBeforeAction(Action action);

        /// <summary>
        /// アクション前にフェードアウトを実行します
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        UniTask FadeOutBeforeAction(Action action);
    }
}