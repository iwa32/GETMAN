using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    [Header("フェード")]
    [SerializeField]
    private FadeImage fade;

    private bool firstPush = false;
    private bool goNextScene = false;


    public void OnClickGameStart()
    {
        //二重送信防止
        if (firstPush) return;
        fade.StartFadeOut();
        firstPush = true;
    }

    
    // Update is called once per frame
    void Update()
    {
        if(!goNextScene && fade.IsCompFadeOut)
        {
            SceneManager.LoadScene("Stage1");
            goNextScene = true;
        }
    }
}
