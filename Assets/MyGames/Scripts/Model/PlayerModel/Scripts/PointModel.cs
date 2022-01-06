using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace PlayerModel
{
    public class PointModel : IPointModel
    {
        public IReadOnlyReactiveProperty<int> Point => _point;

        IntReactiveProperty _point = new IntReactiveProperty();
        int _initialPoint;

        internal PointModel(int point)
        {
            _point.Value = point;
            _initialPoint = point;
        }

        public void AddPoint(int point)
        {
            _point.Value += point;
        }

        public void ResetPoint()
        {
            _point.Value = _initialPoint;
        }
    }
}