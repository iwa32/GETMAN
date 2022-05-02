using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WeaponInvokerUtility
{
    /// <summary>
    /// レーザーキャノンの共通機能クラス
    /// </summary>
    public class LaserCannonUtility : MonoBehaviour
    {
        [SerializeField]
        [Header("レーザーの射出位置を設定")]
        float _shootingHeight = 1.5f;

        [SerializeField]
        [Header("プールに保持するレーザーの生成数を設定")]
        int _maxObjectCount = 3;

        Transform _shooterTransform;

        public int MaxObjectCount => _maxObjectCount;

        /// <summary>
        /// 射出位置を取得します
        /// </summary>
        /// <returns></returns>
        public Vector3 GetShootPosition(Transform shooter)
        {
            //一度だけ設定
            if (_shooterTransform == null && shooter != null)
                _shooterTransform = shooter;

            Vector3 shootPos = _shooterTransform.position;
            //+_shooterTransform.forward
            shootPos.y = _shootingHeight;

            return shootPos;
        }
    }
}
