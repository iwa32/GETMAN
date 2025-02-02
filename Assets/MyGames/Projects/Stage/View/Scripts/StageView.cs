using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StageObject;
using System.Linq;

namespace StageView
{
    public class StageView : MonoBehaviour
    {
        [SerializeField]
        [Header("プレイヤーの開始地点を設定")]
        Transform _playerStartingPoint;

        [SerializeField]
        [Header("エネミーの出現地点を持つ親オブジェクトを設定")]
        Transform _parentOfEnemyAppearance;

        [SerializeField]
        [Header("ポイントアイテムの出現地点を持つ親オブジェクトを設定")]
        Transform _parentOfPointItemAppearance;

        [SerializeField]
        [Header("巡回可能地点を持つ親オブジェクトを設定")]
        Transform _parentOfPatrol;

        EnemyInterferencePoint[] _enemyAppearancePoints;
        Transform[] _pointItemAppearancePoints;
        EnemyInterferencePoint[] _patrolPoints;
        Vector3 _prevPointItemPosition;//前回のポイントアイテム出現位置

        public Transform PlayerStartingPoint => _playerStartingPoint;

        /// <summary>
        /// ステージの初期化を行います
        /// </summary>
        public void InitializeStagePoints()
        {
            //エネミー、ポイントアイテム、巡回の地点を設定します
            SetPoints(_parentOfEnemyAppearance, ref _enemyAppearancePoints);
            SetPoints(_parentOfPointItemAppearance, ref _pointItemAppearancePoints);
            SetPoints(_parentOfPatrol, ref _patrolPoints);
        }

        void SetPoints<T>(Transform from, ref T[] to)
        {
            if (from == null) return;
            //初期化時に値渡しになってしまうためrefで参照を初期化させます
            to = new T[from.childCount];

            for (int i = 0; i < to.Length; i++)
            {
                to[i] = from.GetChild(i).GetComponent<T>();
            }
        }

        /// <summary>
        /// エネミーの出現地点を取得します
        /// </summary>
        /// <param name="enemyTransform"></param>
        public Transform GetEnemyAppearancePoint(EnemyType enemyType)
        {
            //エネミーのタイプを渡して対応したランダムな出現地点を返します
            EnemyInterferencePoint[] targetPoints
                = _enemyAppearancePoints.Where(point => point.EnemyType == enemyType).ToArray();

            if (targetPoints.Length == 0)
                Debug.Log("エネミーが配置できません");

            Transform randomPoint
                = GetRandomAppearancePointFor(targetPoints).transform;
            return randomPoint;
        }

        /// <summary>
        /// エネミーのパトロール地点を取得します
        /// </summary>
        /// <param name="enemyType"></param>
        /// <returns></returns>
        public Transform[] GetEnemyPatrolPoints(EnemyType enemyType)
        {
            //エネミーのタイプに応じたパトロール地点を取得します
            Transform[] targetPoints
                = _patrolPoints
                .Where(point => point.EnemyType == enemyType)
                .Select(point => point.transform)
                .ToArray();

            return targetPoints;
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
        T GetRandomAppearancePointFor<T>(T[] appearancePoints)
        {
            return appearancePoints[Random.Range(0, appearancePoints.Length)];
        }
    }
}
