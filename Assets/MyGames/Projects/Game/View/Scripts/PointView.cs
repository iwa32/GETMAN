using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameView
{
    public class PointView : MonoBehaviour
    {
        [SerializeField]
        [Header("ポイントのUIプレハブを設定します")]
        GameObject pointPrefab;

        [SerializeField]
        [Header("ポイントUIの格納場所を設定")]
        Transform pointTransform;

        /// <summary>
        /// ポイントのセット
        /// </summary>
        public void SetPointGauge(int point)
        {
            //ポイントを一旦削除
            for (int i = 0; i < pointTransform.childCount; i++)
            {
                Destroy(pointTransform.GetChild(i).gameObject);
            }

            //追加していく
            for (int i = 0; i < point; i++)
            {
                Instantiate(pointPrefab, pointTransform);
            }
        }
    }
}
