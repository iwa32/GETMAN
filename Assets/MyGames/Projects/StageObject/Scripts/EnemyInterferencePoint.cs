using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StageObject
{
    /// <summary>
	/// エネミーの干渉点
	/// </summary>
    public class EnemyInterferencePoint : MonoBehaviour
    {
        [SerializeField]
        [Header("干渉するエネミーの種類を設定します")]
        EnemyType _enemyType;

        public EnemyType EnemyType => _enemyType;
    }
}