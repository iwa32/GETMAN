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
        [Header("フィールドオブジェクトの出現地点を設定")]
        GameObject[] _fieldObjectAppearancePoints;

        /// <summary>
		/// プレイヤーをスタート地点に設定します
		/// </summary>
        public void SetPlayerToStartPoint(Transform playerTransform)
        {
            playerTransform.position = transform.position;
        }
    }
}
