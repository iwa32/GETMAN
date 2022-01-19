using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StageDataList", menuName = "ScriptableObject/Create StageDataList")]
public class StageDataList : ScriptableObject
{
    [SerializeField]
    List<StageData> _stageDataList = new List<StageData>();

    public List<StageData> GetStageDataList => _stageDataList;

    /// <summary>
    /// idからステージデータを取得します
    /// </summary>
    /// <returns></returns>
    public StageData GetStageById(int id)
    {
        try
        {
            return _stageDataList.Find(stage => stage.StageId == id);
        }
        catch
        {
            Debug.Log("ステージが見つかりませんでした");
            return null;
        }
    }
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
    [Tooltip("エネミーの生成間隔(○秒ごとに生成)")]
    float _enemyAppearanceInterval;

    [SerializeField]
    [Tooltip("ポイントアイテムの生成間隔(○秒ごとに生成)")]
    float _pointItemAppearanceInterval;

    [SerializeField]
    [Tooltip("出現エネミーを設定")]
    EnemyType[] _appearingEnemies;

    [SerializeField]
    AudioClip _stageBgm;

    [SerializeField]
    [Tooltip("エネミーの最大出現数")]
    int _maxEnemyCount;

    [SerializeField]
    [Tooltip("ステージのPrefabを設定")]
    StageView.StageView _stagePrefab;


    public int StageId => _stageId;
    public int ClearPointCount =>  _clearPointCount;
    public float EnemyAppearanceInterval => _enemyAppearanceInterval;
    public float PointItemAppearanceInterval => _pointItemAppearanceInterval;
    public EnemyType[] AppearingEnemies => _appearingEnemies;
    public AudioClip StageBgm => _stageBgm;
    public int MaxEnemyCount => _maxEnemyCount;
    public StageView.StageView StagePrefab => _stagePrefab;
}