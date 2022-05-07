using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameView
{
    public class PointView : MonoBehaviour
    {
        [SerializeField]
        [Header("ポイントのUIプレハブを設定します")]
        GameObject _pointPrefab;

        [SerializeField]
        [Header("獲得ポイントUIの格納場所を設定")]
        Transform _gettingPointTransform;

        [SerializeField]
        [Header("残りポイント数のUIの格納場所を設定")]
        Transform _remainingPointTransform;

        /// <summary>
        /// 獲得ポイントのセット
        /// </summary>
        public void SetGettingPointGauge(int point)
        {
            //ポイントを一旦削除
            for (int i = 0; i < _gettingPointTransform.childCount; i++)
            {
                Destroy(_gettingPointTransform.GetChild(i).gameObject);
            }

            CreatePointItem(point, _gettingPointTransform);
        }

        /// <summary>
        /// 残りポイント数のセット
        /// </summary>
        /// <param name="point"></param>
        public void SetRemainingPointGauge(int point)
        {
            CreatePointItem(point, _remainingPointTransform);
        }

        /// <summary>
        /// ポイントアイテムをuiに設定
        /// </summary>
        /// <param name="point"></param>
        /// <param name="targetTransform"></param>
        void CreatePointItem(int point, Transform targetTransform)
        {
            //追加していく
            for (int i = 0; i < point; i++)
            {
                Instantiate(_pointPrefab, targetTransform);
            }
        }
    }
}
