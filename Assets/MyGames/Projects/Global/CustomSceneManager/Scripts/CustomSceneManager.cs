using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fade;
using Cysharp.Threading.Tasks;
using Zenject;

namespace CustomSceneManager
{
    public class CustomSceneManager : MonoBehaviour, ICustomSceneManager
    {
        IFade _fade;

        [Inject]
        public void Construct(IFade fade)
        {
            _fade = fade;
        }

        public void LoadScene(SceneType sceneName)
        {
            //引数のあるAction型を呼び出し先に渡すための整形処理
            Action action = () =>
            {
                SceneManager.LoadScene(CommonAttribute.GetStringValue(sceneName));
            };

            //フェードアウト後シーンを読み込みます
            _fade.FadeOutBeforeAction(action).Forget();
        }
    }
}
