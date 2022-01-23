using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CustomSceneManager
{
    public class CustomSceneManager : MonoBehaviour, ICustomSceneManager
    {
        public void LoadScene(SceneType sceneName)
        {
            //todo フェードを出現させる
            SceneManager.LoadScene(CommonAttribute.GetStringValue(sceneName));
        }
    }
}
