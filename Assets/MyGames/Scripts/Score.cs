using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour, IUpdateableUI
{
    private Text scoreText;
    private int oldScoreNum;

    // Start is called before the first frame update
    void Start()
    {
        scoreText = GetComponent<Text>();
        if(GameManager.instance != null)
        {
            UpdateUiText();
        }
        else
        {
            Debug.Log("GameManagerが設定されていません。");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(oldScoreNum != GameManager.instance.ScoreNum)
        {
            UpdateUiText();
            oldScoreNum = GameManager.instance.ScoreNum;
        }
    }

    /// <summary>
    /// Scoreの更新
    /// </summary>
    public void UpdateUiText()
    {
        scoreText.text = GameManager.instance.ScoreNum.ToString();
    }
}
