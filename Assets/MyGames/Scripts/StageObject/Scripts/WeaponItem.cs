using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponItem : MonoBehaviour,
    IWeaponItem
{
    #region//インスペクターで設定
    [Header("獲得スコア")]
    [SerializeField]
    int _score;

    public int Score => _score;

    public int Power => throw new System.NotImplementedException();

    public int Id => throw new System.NotImplementedException();
    #endregion

    public void Destroy()
    {
        throw new System.NotImplementedException();
    }
}
