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

        Vector3 _prevPointItemPosition;//前回のポイントアイテム出現位置

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

            //出現箇所が1つなら再取得しない
            if (_pointItemAppearancePoints.Length == 1) return randomPoint;

            //前回と同じ位置なら取得しなおします
            while (randomPoint.position == _prevPointItemPosition)
            {
                randomPoint = GetRandomAppearancePointFor(_pointItemAppearancePoints);
            }
            _prevPointItemPosition = randomPoint.position;
            
            
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
