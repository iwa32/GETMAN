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

        /// <summary>
		/// プレイヤーをスタート地点に設定します
		/// </summary>
        public void SetPlayerToStartPoint(Transform playerTransform)
        {
            playerTransform.position = _playerStartingPoint.position;
        }

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
        /// ポイントアイテムをランダムな出現地点に設定します
        /// </summary>
        /// <param name="enemyTransform"></param>
        public void SetPointItemToRandomAppearancePoint(Transform pointItemTransform)
        {
            Transform randomPoint = GetRandomAppearancePointFor(_pointItemAppearancePoints);
            SetTargetToAppearancePoint(pointItemTransform, randomPoint);
        }

        /// <summary>
        /// ランダムな出現地点を取得します
        /// </summary>
        /// <param name="AppearancePoints"></param>
        Transform GetRandomAppearancePointFor(GameObject[] appearancePoints)
        {
            return appearancePoints[Random.Range(0, appearancePoints.Length)].transform;
        }

        /// <summary>
        /// 対象を出現地点に設定します
        /// </summary>
        /// <param name="target"></param>
        /// <param name="appearancePoint"></param>
        void SetTargetToAppearancePoint(Transform target, Transform appearancePoint)
        {
            target.position = appearancePoint.position;
            target.rotation = appearancePoint.rotation;
        }
    }
}
