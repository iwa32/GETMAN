using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpView : MonoBehaviour
{
    [SerializeField]
    [Header("HPのUIプレハブを設定")]
    private GameObject hpPrefab;//hpUIのプレハブ

    /// <summary>
    /// HPの設定を行います
    /// </summary>
    /// <param name="hp"></param>
    public void SetHpGauge(int hp)
    {
        //体力を一旦削除
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        //現在の体力数分のゲージを作成
        for (int i = 0; i < hp; i++)
        {
            Instantiate(hpPrefab, transform);//prefab, 配置場所
        }
    }

    //ダメージ分だけ削除
    public void ReduceHp(int damage)
    {
        for (int i = 0; i < damage; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
}
