using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [HideInInspector]
    public static GameManager instance;

    [Header("ポイント")]
    private int _pointNum;
    [Header("HP")]
    private int _heartNum;
    [Header("スコア")]
    private int _scoreNum;

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

}
