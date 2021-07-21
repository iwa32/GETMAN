using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region//インスペクターで設定
    [HideInInspector]
    public static GameManager instance;
    [Header("初期時のHP")]
    public int defaultHeartNum;
    [Header("初期時の制限時間を設定")]
    public float defaultLimitTimer = 60.0f;
    [Header("クリアポイント数")]
    public int clearPointNum = 3;
    #endregion

    [Header("ポイント")]
    private int _pointNum;
    [Header("HP")]
    private int _heartNum;
    [Header("スコア")]
    private int _scoreNum;
    [Header("ステージ")]
    private int _stageNum;
    [Header("制限時間")]
    private float _limitTimer = 60.0f;

    #region//Component
    private AudioSource audioSource;
    #endregion

    #region//フラグ
    public bool isGameOver;
    private bool isStageClear;
    #endregion

    #region//getter, setter
    public int PointNum
    {
        get { return _pointNum; }
        set { _pointNum = value; }
    }
    public int HeartNum
    {
        get { return _heartNum; }
        set { _heartNum = value; }
    }
    public int ScoreNum
    {
        get { return _scoreNum; }
        set { _scoreNum = value; }
    }
    public int StageNum
    {
        get { return _stageNum; }
        set { _stageNum = value; }
    }
    public float LimitTimer
    {
        get { return _limitTimer; }
        set { _limitTimer = value; }
    }
    #endregion

    private void Awake()
    {
        //シングルトンで実装する
        if(instance == null)
        {
            instance = this;
            //シーンを跨いでも削除しない
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (PointNum == clearPointNum)
        {
            isStageClear = true;
            Debug.Log("ステージクリア");
        }

        CheckIfGameOver();
        CountTimer();
    }

    /// <summary>
    /// ゲームオーバーかチェックする
    /// </summary>
    private void CheckIfGameOver()
    {
        if (HeartNum == 0 || LimitTimer == 0.0f )
        {
            isGameOver = true;
        }
    }

    /// <summary>
    /// 最初から始める時の処理
    /// </summary>
    public void RetryGame()
    {
        isGameOver = false;
        ScoreNum = 0;
        StageNum = 1;
        PointNum = 0;
        HeartNum = defaultHeartNum;
        LimitTimer = defaultLimitTimer;
    }

    /// <summary>
    /// カウントする
    /// </summary>
    private void CountTimer()
    {
        if(LimitTimer > 0)
        {
            LimitTimer -= Time.deltaTime;
        }
        else
        {
            LimitTimer = 0.0f;
        }
    }

    /// <summary>
    /// SEを再生する
    /// </summary>
    /// <param name="clip"></param>
    public void PlaySE(AudioClip clip)
    {
        if (clip == null) return;
        audioSource.PlayOneShot(clip);
    }

    /// <summary>
    /// ポイントの増加
    /// </summary>
    public void AddPointNum(int num)
    {
        PointNum += num;
    }


    /// <summary>
    /// スコアの増加
    /// </summary>
    public void AddScoreNum(int num)
    {
        ScoreNum += num;
    }

    /// <summary>
    /// HPを減らす
    /// </summary>
    public void ReduceHeartNum()
    {
        if(HeartNum > 0)
        {
            --HeartNum;
        }
        else
        {
            HeartNum = 0;
        }
    }
}
