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
        [Header("エネミーの出現地点を持つ親オブジェクトを設定")]
        Transform _parentEnemyAppearancePoints;

        [SerializeField]
        [Header("ポイントアイテムの出現地点を持つ親オブジェクトを設定")]
        Transform _parentPointItemAppearancePoints;

        [SerializeField]
        [Header("巡回可能地点を持つ親オブジェクトを設定")]
        Transform _parentPatrolPoints;

        Transform[] _enemyAppearancePoints;
        Transform[] _pointItemAppearancePoints;
        Transform[] _patrolPoints;
        Vector3 _prevPointItemPosition;//前回のポイントアイテム出現位置

        public Transform PlayerStartingPoint => _playerStartingPoint;
        public Transform[] PatrollPoints => _patrolPoints;

        private void Awake()
        {
            InitializeStagePoints();
        }

        /// <summary>
        /// ステージの初期化を行います
        /// </summary>
        void InitializeStagePoints()
        {
            //エネミー、ポイントアイテム、巡回地点を設定します
            SetPoints(_parentEnemyAppearancePoints, ref _enemyAppearancePoints);
            SetPoints(_parentPointItemAppearancePoints, ref _pointItemAppearancePoints);
            SetPoints(_parentPatrolPoints, ref _patrolPoints);
        }

        void SetPoints(Transform from, ref Transform[] to)
        {
            //初期化時に値渡しになってしまうためrefで参照を初期化させます
            to = new Transform[from.childCount];

            for (int i = 0; i < to.Length; i++)
            {
                to[i] = from.GetChild(i);
            }
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
        Transform GetRandomAppearancePointFor(Transform[] appearancePoints)
        {
            return appearancePoints[Random.Range(0, appearancePoints.Length)];
        }
    }
}
