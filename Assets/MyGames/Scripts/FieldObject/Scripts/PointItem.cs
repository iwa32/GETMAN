using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointItem : MonoBehaviour,
    IPointItem
{
    #region//インスペクターで設定
    [Header("獲得ポイント数")]
    [SerializeField]
    int _point = 1;

    [Header("獲得スコア")]
    [SerializeField]
    int _score;

    public int Point => _point;
    public int Score => _score;
    #endregion

    //プレイヤーに接触したら破棄する
    public void Destroy()
    {
        //todoseを鳴らす
        Destroy(gameObject);
    }
}
