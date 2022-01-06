using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointItem : MonoBehaviour
{
    #region//インスペクターで設定
    public AudioClip getSE;
    [Header("獲得ポイント数")]
    public int pointNum = 1;
    [Header("獲得スコア")]
    public int scoreNum;
    #endregion

    [SerializeField]
    private PlayerTriggerCheck trigger;

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
                GameManager.instance.AddPointNum(pointNum);
                GameManager.instance.AddScoreNum(scoreNum);
            }
            //消滅
            Destroy(gameObject);
        }
    }
}
