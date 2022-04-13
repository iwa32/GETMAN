using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StageObject
{
    public class PointItem : GetableItem, IPoint
    {
        #region//インスペクターで設定
        [Header("獲得ポイント数")]
        [SerializeField]
        int _point = 1;

        public int Point => _point;
        #endregion

        //プレイヤーに接触したら破棄する
        public override void Destroy()
        {
            gameObject.SetActive(false);
        }
    }
}