using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageCtrl : MonoBehaviour
{
    [Header("プレイヤーオブジェクト")]
    public GameObject playerObj;
    [Header("プレイヤー待機場所")]
    public GameObject playerWaitLocationObj;

    private PlayerController p;
    // Start is called before the first frame update
    void Start()
    {
        if(playerObj != null)
        {
            //コンティニュー地点へプレイヤーを設置
            MovingPlayerIntoWaitPosition();
            p = playerObj.GetComponent<PlayerController>();
        }
        else
        {
            Debug.Log("設定が足りていません");
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if(p != null && p.IsContinueWaiting())
        {
            //プレイヤーがコンティニュー待機中なら地点へ設置
            MovingPlayerIntoWaitPosition();
            p.ContinuePlayer();
        }
    }

    /// <summary>
    /// プレイヤーを待機地点へ移動
    /// </summary>
    void MovingPlayerIntoWaitPosition()
    {
        playerObj.transform.position = playerWaitLocationObj.transform.position;
    }
}
