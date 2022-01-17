using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StageDataList", menuName = "ScriptableObject/Create StageDataList")]
public class StageDataList : ScriptableObject
{
    [SerializeField]
    List<StageData> _stageDataList = new List<StageData>();

    public List<StageData> GetStageDataList => _stageDataList;
}

[System.Serializable]
public class StageData
{

    [SerializeField]
    int _stageId;

    [SerializeField]
    [Tooltip("クリアする必要ポイント数")]
    int _clearPointCount;

    [SerializeField]
    [Tooltip("フィールドオブジェクトの生成間隔(○秒ごとに生成)")]
    float _filedObjectAppearanceInterval;

    [SerializeField]
    [Tooltip("出現モンスターを設定")]
    EnemyType[] _appearingMonsters;

    [SerializeField]
    AudioClip _stageBgm;

    [SerializeField]
    [Tooltip("フィールドオブジェクト最大出現数")]
    int _maxFieldObjectCount;

    [SerializeField]
    [Tooltip("ステージのPrefabを設定")]
    GameObject _stagePrefab;


    public int StageId => _stageId;
    public int ClearPointCount =>  _clearPointCount;
    public float FiledObjectAppearanceInterval => _filedObjectAppearanceInterval;
    public EnemyType[] AppearingMonsters => _appearingMonsters;
    public AudioClip StageBgm => _stageBgm;
    public int MaxFieldObjectCount => _maxFieldObjectCount;
    public GameObject StagePrefab => _stagePrefab;
}