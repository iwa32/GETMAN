using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageCtrl : MonoBehaviour
{
    [Header("プレイヤーオブジェクト")]
    public GameObject playerObj;
    [Header("プレイヤー待機場所")]
    public GameObject playerWaitLocationObj;
    [SerializeField]
    [Header("ゲームオーバUI")]
    private GameObject gameOverObj;

    private PlayerController p;
    private bool doGameOver;


    // Start is called before the first frame update
    void Start()
    {
        if(playerObj != null && playerWaitLocationObj != null && gameOverObj != null)
        {
            //最初は非表示
            gameOverObj.SetActive(false);
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
        if(GameManager.instance.isGameOver && !doGameOver)
        {
            GameOver();
        }
        else if(p != null && p.IsContinueWaiting())
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

    /// <summary>
    /// ゲームオーバーにする
    /// </summary>
    void GameOver()
    {
        gameOverObj.SetActive(true);
        doGameOver = true;
    }
}
