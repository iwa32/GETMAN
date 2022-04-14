using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EP = EnemyPresenter;

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
    [System.Serializable]
    class PointOption
    {
        [SerializeField]
        [Tooltip("ポイントの生成パターン")]
        PointGenerationType _pointGenerationType;

        [SerializeField]
        [Tooltip("クリアする必要ポイント数")]
        int _clearPointCount;

        [SerializeField]
        [Tooltip("ポイントアイテムの生成間隔(○秒ごとに生成)")]
        float _pointItemAppearanceInterval;

        public PointGenerationType PointGenerationType => _pointGenerationType;
        public int ClearPointCount => _clearPointCount;
        public float PointItemAppearanceInterval => _pointItemAppearanceInterval;
    }

    [System.Serializable]
    class EnemyOption
    {
        [SerializeField]
        [Tooltip("エネミーの最大出現数")]
        int _maxEnemyCount;

        [SerializeField]
        [Tooltip("エネミーの生成間隔(○秒ごとに生成)")]
        float _enemyAppearanceInterval;

        [SerializeField]
        [Tooltip("出現エネミーのプレハブを設定")]
        EP.EnemyPresenter[] _enemyPrefabs;

        public int MaxEnemyCount => _maxEnemyCount;
        public float EnemyAppearanceInterval => _enemyAppearanceInterval;
        public EP.EnemyPresenter[] AppearingEnemyPrefabs => _enemyPrefabs;
    }

    [SerializeField]
    [Tooltip("ステージ番号")]
    int _stageId;

    [SerializeField]
    [Tooltip("ステージの制限時間を設定")]
    int _stageLimitCountTime = 60;

    [SerializeField]
    [Tooltip("Bgmの種類を設定")]
    BgmType _bgmType;

    [SerializeField]
    [Tooltip("ステージのPrefabを設定")]
    StageView.StageView _stagePrefab;

    [SerializeField]
    [Tooltip("ポイントの設定")]
    PointOption _pointOption;

    [SerializeField]
    [Tooltip("エネミーの設定")]
    EnemyOption _enemyOption;


    public int StageId => _stageId;
    public int StageLimitCountTime => _stageLimitCountTime;
    public BgmType BgmType => _bgmType;
    public StageView.StageView StagePrefab => _stagePrefab;

    //ポイント
    public PointGenerationType PointGenerationType => _pointOption.PointGenerationType;
    public int ClearPointCount => _pointOption.ClearPointCount;
    public float PointItemAppearanceInterval => _pointOption.PointItemAppearanceInterval;
    //エネミー
    public int MaxEnemyCount => _enemyOption.MaxEnemyCount;
    public float EnemyAppearanceInterval => _enemyOption.EnemyAppearanceInterval;
    public EP.EnemyPresenter[] AppearingEnemyPrefabs => _enemyOption.AppearingEnemyPrefabs;

}