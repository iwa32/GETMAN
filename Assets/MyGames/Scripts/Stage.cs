using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stage : MonoBehaviour
{
    private Text stageNumText;
    private int oldStageNum;//ステージ番号のキャッシュ

    // Start is called before the first frame update
    void Start()
    {
        stageNumText = GetComponent<Text>();
        if (GameManager.instance != null)
        {
            UpdateStageNumText();
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
            UpdateStageNumText();
            oldStageNum = GameManager.instance.StageNum;
        }
    }

    /// <summary>
    /// Stage番号の更新
    /// </summary>
    private void UpdateStageNumText()
    {
        stageNumText.text = GameManager.instance.StageNum.ToString();
    }
}
