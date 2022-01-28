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
    }
}