using UnityEngine;
/// <summary>
/// ポイントの生成パターンを管理
/// </summary>
public enum PointGenerationType
{
    [Tooltip("ランダムに生成")]
    RANDOM_GENERATION,
    [Tooltip("順番に生成")]
    ORDER_GENERATION,
    [Tooltip("生成しない")]
    NO_GENERATION
}
