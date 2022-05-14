using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SEDataList", menuName = "ScriptableObject/Create SEDataList")]
public class SEDataList : ScriptableObject
{
    [SerializeField]
    List<SEData> _seDataList = new List<SEData>();

    public List<SEData> GetSEDataList => _seDataList;

    /// <summary>
    /// enumからseを取得します
    /// </summary>
    /// <returns></returns>
    public SEData FindSEDataByType(SEType type)
    {
        try
        {
            return _seDataList.Find(seData => seData.Type == type);
        }
        catch
        {
            Debug.Log("SEが見つかりませんでした");
            return null;
        }
    }
}

[Serializable]
public class SEData
{
    [SerializeField]
    [Header("SEの種類")]
    SEType _type;

    [SerializeField]
    [Header("SEを設定")]
    AudioClip _clip;

    public SEType Type => _type;
    public AudioClip Clip => _clip;
}
