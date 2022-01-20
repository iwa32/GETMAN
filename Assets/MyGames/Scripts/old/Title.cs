using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Title : MonoBehaviour
{
    [Header("フェード")]
    [SerializeField]
    private FadeImage fade;
    [Header("スタートボタンのテキスト")]
    [SerializeField]
    private Text startButtonText;
    [Header("点滅の頻度")]
    public float speed = 1.0f;
    [Header("クリック時のSE")]
    [SerializeField]
    private AudioClip clickSE;

    private float blinkTimer;
    private float stagingTimer;
    private bool doStaging;
    private bool firstPush = false;
    private bool goNextScene = false;

    private void Start()
    {
        //if (GameManager.instance == null)
        //{
        //    Debug.Log("ゲームマネージャーがセットされていません");
        //}
    }

    // Update is called once per frame
    void Update()
    {
        if (doStaging && !fade.IsCompFadeOut)
        {
            fade.StartFadeOut();
        }

        if (!goNextScene && fade.IsCompFadeOut)
        {
            SceneManager.LoadScene("Stage1");
            //GameManager.instance.isGameStart = true;
            goNextScene = true;
        }
    }

    public void OnClickGameStart()
    {
        //二重送信防止
        if (firstPush) return;
        firstPush = true;
        //GameManager.instance.PlaySE(clickSE);
        StartCoroutine("IntervalBlink");
    }

    /// <summary>
    /// タイトルの点滅
    /// </summary>
    /// <returns></returns>
    IEnumerator IntervalBlink()
    {
        while(stagingTimer < 1.0f)
        {
            startButtonText.color = GetAlphaColor(startButtonText.color);
            stagingTimer += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        doStaging = true;//演出終わり
    }

    /// <summary>
    /// アルファ値を取得
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    Color GetAlphaColor(Color color)
    {
        blinkTimer += Time.deltaTime * 5.0f * speed;
        color.a = Mathf.Sin(blinkTimer);
        return color;
    }
}
