using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StageObject
{
    /// <summary>
	/// エネミーの生成地点の設定
	/// </summary>
    public class EnemyAppearancePoint : MonoBehaviour
    {
        [SerializeField]
        [Header("生成地点に出現するエネミーを設定します")]
        EnemyType _enemyType;

        public EnemyType EnemyType => _enemyType;
    }
}