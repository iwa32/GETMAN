using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StageView
{
    public class StageView : MonoBehaviour
    {
        [SerializeField]
        [Header("プレイヤーの開始地点を設定")]
        GameObject _playerStartingPoint;

        [SerializeField]
        [Header("エネミーの出現地点を設定")]
        GameObject[] _enemyAppearancePoints;

        /// <summary>
		/// プレイヤーをスタート地点に設定します
		/// </summary>
        public void SetPlayerToStartPoint(Transform playerTransform)
        {
            playerTransform.position = transform.position;
        }

        /// <summary>
        /// エネミーをランダムな出現地点に設定します
        /// </summary>
        /// <param name="enemyTransform"></param>
        public void SetEnemyToRandomAppearancePoint(Transform enemyTransform)
        {
            GameObject randomPoints = _enemyAppearancePoints[Random.Range(0, _enemyAppearancePoints.Length)];
            enemyTransform.position = randomPoints.transform.position;
            enemyTransform.rotation = randomPoints.transform.rotation;
        }
    }
}
