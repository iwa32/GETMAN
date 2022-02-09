using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StageView
{
    public class StageView : MonoBehaviour
    {
        [SerializeField]
        [Header("プレイヤーの開始地点を設定")]
        Transform _playerStartingPoint;

        [SerializeField]
        [Header("エネミーの出現地点を設定")]
        GameObject[] _enemyAppearancePoints;

        [SerializeField]
        [Header("ポイントアイテムの出現地点を設定")]
        GameObject[] _pointItemAppearancePoints;

        public Transform PlayerStartingPoint => _playerStartingPoint;

        /// <summary>
        /// エネミーの出現地点を取得します
        /// </summary>
        /// <param name="enemyTransform"></param>
        public Transform GetEnemyAppearancePoint()
        {
            Transform randomPoint = GetRandomAppearancePointFor(_enemyAppearancePoints);
            return randomPoint;
        }

        /// <summary>
        /// ポイントアイテムの出現地点を取得します
        /// </summary>
        /// <param name="enemyTransform"></param>
        public Transform GetPointItemAppearancePoint()
        {
            Transform randomPoint = GetRandomAppearancePointFor(_pointItemAppearancePoints);
            return randomPoint;
        }

        /// <summary>
        /// ランダムな出現地点を取得します
        /// </summary>
        /// <param name="AppearancePoints"></param>
        Transform GetRandomAppearancePointFor(GameObject[] appearancePoints)
        {
            return appearancePoints[Random.Range(0, appearancePoints.Length)].transform;
        }
    }
}
