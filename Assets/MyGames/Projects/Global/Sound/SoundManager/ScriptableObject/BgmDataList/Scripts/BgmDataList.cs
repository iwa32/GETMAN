using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BgmDataList", menuName = "ScriptableObject/Create BgmDataList")]
public class BgmDataList : ScriptableObject
{
    [SerializeField]
    List<BgmData> _bgmDataList = new List<BgmData>();

    public List<BgmData> GetBgmDataList => _bgmDataList;

    /// <summary>
    /// enumからBgmのクリップを取得します
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public AudioClip FindBgmClipByType(BgmType type)
    {
        try
        {
            return _bgmDataList
                .Find(bgmData => bgmData.Type == type)
                .Clip;
        }
        catch
        {
            Debug.Log("Bgmが見つかりませんでした");
            return null;
        }
    }
}

[Serializable]
public class BgmData
{
    [SerializeField]
    [Header("Bgmの種類")]
    BgmType _type;

    [SerializeField]
    [Header("Bgmのデータを設定")]
    AudioClip _clip;

    public BgmType Type => _type;
    public AudioClip Clip => _clip;
}
