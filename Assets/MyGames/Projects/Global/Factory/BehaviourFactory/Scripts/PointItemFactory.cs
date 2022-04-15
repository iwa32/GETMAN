using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StageObject;
using ObjectPool;
using Zenject;

namespace BehaviourFactory
{
    public class PointItemFactory : BehaviourFactory<PointItem>
    {
        [SerializeField]
        [Header("ポイントアイテムを設定します")]
        PointItem _pointItemPrefab;

        IPointItemPool _pointItemPool;

        [Inject]
        public void Construct(IPointItemPool pointItemPool)
        {
            _pointItemPool = pointItemPool;
        }

        public void SetPointItem(int maxPointItemCount)
        {
            _pointItemPool.CreatePool(_pointItemPrefab, maxPointItemCount);
        }

        public override PointItem Create(PointItem prefab = null)
        {
            return _pointItemPool.GetPool();
        }
    }
}