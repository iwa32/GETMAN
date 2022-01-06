using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointGauge : MonoBehaviour
{
    [SerializeField]
    private GameObject pointObj;
    private int oldPointNum;//ポイントのキャッシュ

    // Start is called before the first frame update
    void Start()
    {
        if(GameManager.instance == null)
        {
            Debug.Log("GameManagerが設定されていません");
        }
    }

    // Update is called once per frame
    void Update()
    {
        //差分があったら更新
        if (oldPointNum != GameManager.instance.PointNum)
        {
            oldPointNum = GameManager.instance.PointNum;
            SetPointGauge(oldPointNum);
        }
    }

    /// <summary>
    /// ポイントのセット
    /// </summary>
    void SetPointGauge(int point)
    {
        //ポイントを一旦削除
        for(int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        //追加していく
        for (int i = 0; i < point; i++)
        {
            Instantiate(pointObj, transform);
        }
    }
}
