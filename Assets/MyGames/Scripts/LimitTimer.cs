using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LimitTimer : MonoBehaviour
{
    private Text limitTimerText;
    private float oldLimitTimer;//時間のキャッシュ

    // Start is called before the first frame update
    void Start()
    {
        limitTimerText = GetComponent<Text>();
        if (GameManager.instance != null)
        {
            UpdateLimitTimer();
        }
        else
        {
            Debug.Log("GameManagerが設定されていません。");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (oldLimitTimer != GameManager.instance.limitTimer)
        {
            UpdateLimitTimer();
            oldLimitTimer = GameManager.instance.limitTimer;
        }
    }

    /// <summary>
    /// Scoreの更新
    /// </summary>
    void UpdateLimitTimer()
    {
        limitTimerText.text = "Timer " + GameManager.instance.limitTimer.ToString("F1");
    }
}
