using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeImage : MonoBehaviour
{
    [Header("最初からフェードインが完了しているかどうか")]
    public bool firstFadeInComp;

    private Image img;
    private float fadeTimer;
    private int frameCount;//フレームをカウント
    private bool isFadeIn;
    private bool _isCompFadeIn;
    private bool isFadeOut;
    private bool _isCompFadeOut;

    public bool IsCompFadeIn
    {
        get { return _isCompFadeIn; }
        set { _isCompFadeIn = value; }
    }

    public bool IsCompFadeOut
    {
        get { return _isCompFadeOut; }
        set { _isCompFadeOut = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        img = GetComponent<Image>();
        if(firstFadeInComp)
        {
            FadeInComplete();
        }
        else
        {
            StartFadeIn();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //2フレーム後に起動※シーン移動は処理が重いので、ある程度待ってから開始したい
        ++frameCount;
        if (frameCount <= 2) return;

        if(isFadeIn)
        {
            FadeInUpdate();
        }
        else if(isFadeOut)
        {
            FadeOutUpdate();
        }
    }

    /// <summary>
    /// フェードイン開始
    /// </summary>
    public void StartFadeIn()
    {
        if (isFadeIn || isFadeOut) return;
        
        isFadeIn = true;
        IsCompFadeIn = false;
        fadeTimer = 0.0f;
        img.color = new Color(1, 1, 1, 1);
        img.fillAmount = 1;
        img.raycastTarget = true;//当たり判定を入れる
    }

    /// <summary>
    /// フェードアウト開始
    /// </summary>
    public void StartFadeOut()
    {
        if (isFadeIn || isFadeOut) return;

        isFadeOut = true;
        IsCompFadeOut = false;
        fadeTimer = 0.0f;
        img.color = new Color(1, 1, 1, 0);
        img.fillAmount = 0;
        img.raycastTarget = true;//当たり判定を入れる
    }

    /// <summary>
    /// フェードイン中
    /// </summary>
    void FadeInUpdate()
    {
        //フェードイン中
        if (fadeTimer < 1)
        {
            img.color = new Color(1, 1, 1, 1 - fadeTimer);
            img.fillAmount = 1 - fadeTimer;
        }
        //フェードイン完了
        else
        {
            FadeInComplete();
        }
        fadeTimer += Time.deltaTime;
    }

    /// <summary>
    /// フェードアウト中
    /// </summary>
    void FadeOutUpdate()
    {
        //フェードアウト中
        if (fadeTimer < 1)
        {
            img.color = new Color(1, 1, 1, fadeTimer);
            img.fillAmount = fadeTimer;
        }
        //フェードアウト完了
        else
        {
            FadeOutComplete();
        }
        fadeTimer += Time.deltaTime;
    }

    /// <summary>
    /// フェードイン完了
    /// </summary>
    void FadeInComplete()
    {
        img.color = new Color(1, 1, 1, 0);
        img.fillAmount = 0;
        img.raycastTarget = false;
        fadeTimer = 0.0f;
        isFadeIn = false;
        IsCompFadeIn = true;
    }

    /// <summary>
    /// フェードアウト完了
    /// </summary>
    void FadeOutComplete()
    {
        img.color = new Color(1, 1, 1, 1);
        img.fillAmount = 1;
        img.raycastTarget = false;
        fadeTimer = 0.0f;
        isFadeOut = false;
        IsCompFadeOut = true;
    }
}
