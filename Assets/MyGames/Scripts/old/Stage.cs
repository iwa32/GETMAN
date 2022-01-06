using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stage : MonoBehaviour, IUpdateableUI
{
    private Text stageNumText;
    private int oldStageNum;//ステージ番号のキャッシュ

    // Start is called before the first frame update
    void Start()
    {
        stageNumText = GetComponent<Text>();
        if (GameManager.instance != null)
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
        if(oldStageNum != GameManager.instance.StageNum)
        {
            UpdateUiText();
            oldStageNum = GameManager.instance.StageNum;
        }
    }

    /// <summary>
    /// Stage番号の更新
    /// </summary>
    public void UpdateUiText()
    {
        stageNumText.text = GameManager.instance.StageNum.ToString();
    }
}
