using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace PlayerModel
{
    public class PointModel : IPointModel
    {
        IntReactiveProperty _point = new IntReactiveProperty();

        public IReadOnlyReactiveProperty<int> Point => _point;
        

        public void SetPoint(int point)
        {
            _point.Value = point;
        }

        public void AddPoint(int point)
        {
            _point.Value += point;
        }
    }
}