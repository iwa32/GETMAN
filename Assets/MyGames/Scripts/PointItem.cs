using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointItem : MonoBehaviour
{
    #region//インスペクターで設定
    public AudioClip getSE;
    #endregion

    [SerializeField]
    private PlayerTriggerCheck trigger;

    #region//定数
    const int AddPointNum = 1;//ポイント
    const int AddScoreNum = 100;//スコア todo この値をenum化し、アイテムごとにスコアを分けることも検討する
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        if(trigger == null)
        {
            Debug.Log("プレイヤー接触判定がセットされていません");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(trigger.isOn)
        {
            if(GameManager.instance != null && getSE != null)
            {
                GameManager.instance.PlaySE(getSE);
                //ポイント、スコアを付与
                GameManager.instance.PointNum = AddPointNum;
                GameManager.instance.ScoreNum = AddScoreNum;
            }
            //消滅
            Destroy(gameObject);
        }
    }
}
