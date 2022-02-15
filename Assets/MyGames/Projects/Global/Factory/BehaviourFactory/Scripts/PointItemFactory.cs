using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StageObject;

namespace BehaviourFactory
{
    public class PointItemFactory : BehaviourFactory<PointItem>
    {
        [SerializeField]
        [Header("ポイントアイテムを設定します")]
        PointItem _pointItemPrefab;

        public override PointItem Create()
        {
            return Instantiate(_pointItemPrefab, Vector3.zero, Quaternion.identity);
        }
    }
}