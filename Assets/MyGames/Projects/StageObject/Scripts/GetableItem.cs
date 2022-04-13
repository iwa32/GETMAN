using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StageObject
{
    public abstract class GetableItem : MonoBehaviour, IGetableItem
    {
        [SerializeField]
        [Header("獲得スコア")]
        int _score;

        public int Score => _score;

        /// <summary>
        /// 削除処理
        /// </summary>
        public abstract void Destroy();
    }
}
