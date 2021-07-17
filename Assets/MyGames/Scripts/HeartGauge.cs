using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartGauge : MonoBehaviour
{
    [SerializeField]
    private GameObject heartObj;//hpUIのプレハブ

    private void Start()
    {
        if(GameManager.instance != null)
        {
            if(GameManager.instance.defaultHeartNum != 0)
            {
                //初期hpをセット
                SetHeartGauge(GameManager.instance.defaultHeartNum);
            }
            else
            {
                Debug.Log("初期HPを設定し忘れています。");
            }
        }
        else
        {
            Debug.Log("GameManagerが設定されていません");
            Destroy(this);
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
            Instantiate<GameObject>(heartObj, transform);//prefab, 配置場所
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
