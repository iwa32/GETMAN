using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [HideInInspector]
    public static GameManager instance;
    [Header("初期時のHP")]
    public int defaultHeartNum;
    [Header("クリアポイント数")]
    public int clearPointNum = 3;

    [Header("ポイント")]
    private int _pointNum;
    [Header("HP")]
    private int _heartNum;
    [Header("スコア")]
    private int _scoreNum;
    [Header("ステージ")]
    private int _stageNum;

    #region//Component
    private AudioSource audioSource;
    #endregion

    #region//フラグ
    private bool isGameOver;
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
        if(HeartNum != 0)
        {
            --HeartNum;
        }
        else
        {
            //0ならゲームオーバー
            isGameOver = true;
        }
    }
}
