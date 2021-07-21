using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartGauge : MonoBehaviour
{
    [SerializeField]
    private GameObject heartObj;//hpUIのプレハブ
    private int oldHeartNum;//heartのキャッシュ

    private void Start()
    {
        if(GameManager.instance == null)
        {
            Debug.Log("GameManagerが設定されていません");
            Destroy(this);
        }
    }

    private void Update()
    {
        //差分があったら更新
        if(oldHeartNum != GameManager.instance.HeartNum)
        {
            oldHeartNum = GameManager.instance.HeartNum;
            SetHeartGauge(oldHeartNum);
        }
    }

    public void SetHeartGauge(int heart)
    {
        //体力を一旦削除
        for(int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        //現在の体力数分のゲージを作成
        for(int i = 0; i < heart; i++)
        {
            Instantiate(heartObj, transform);//prefab, 配置場所
        }
    }

    //ダメージ分だけ削除
    public void ReduceHeart(int damage)
    {
        for(int i = 0; i < damage; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
}
