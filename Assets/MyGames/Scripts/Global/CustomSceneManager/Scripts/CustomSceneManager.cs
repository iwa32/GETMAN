using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CustomSceneManager
{
    public class CustomSceneManager : MonoBehaviour, ICustomSceneManager
    {
        public void LoadScene(string sceneName)
        {
            //フェード
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
