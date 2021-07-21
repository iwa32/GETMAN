using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageCtrl : MonoBehaviour
{
    [Header("プレイヤーオブジェクト")]
    public GameObject playerObj;
    [Header("プレイヤー待機場所")]
    public GameObject playerWaitLocationObj;
    [SerializeField]
    [Header("ゲームオーバUI")]
    private GameObject gameOverObj;
    [Header("フェードイメージ")]
    public FadeImage fade;
    [Header("ゲームオーバーSE")]
    public AudioClip gameOverSE;
    [Header("リトライボタンSE")]
    public AudioClip retrySE;

    private PlayerController p;
    private int nextStageNum;
    private bool startFade;
    private bool doGameOver;
    private bool retryGame;//リトライ中か
    private bool doSceneChange;//シーン切り替え中か


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

        //ステージを切り替える
        if(fade != null && startFade && !doSceneChange)
        {
            if (fade.IsCompFadeOut == false) return;

            //ゲームリトライ
            if(retryGame)
            {
                GameManager.instance.RetryGame();
            }
            else
            {
                GameManager.instance.StageNum = nextStageNum;
            }
            SceneManager.LoadScene("Stage" + nextStageNum);
            doSceneChange = true;
        }

    }

    /// <summary>
    /// 最初から始める
    /// </summary>
    public void Retry()
    {
        ChangeScene(1);
        GameManager.instance.PlaySE(retrySE);
        retryGame = true;
    }

    /// <summary>
    /// ステージを切り替えます
    /// </summary>
    /// <param name="num">ステージ番号</param>
    public void ChangeScene(int num)
    {
        if (fade == null) return;

        nextStageNum = num;
        fade.StartFadeOut();
        startFade = true;
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
        GameManager.instance.PlaySE(gameOverSE);
        doGameOver = true;
    }
}
