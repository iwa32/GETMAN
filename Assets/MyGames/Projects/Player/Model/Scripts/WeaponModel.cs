using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace PlayerModel
{
    public class WeaponModel : IWeaponModel
    {
        public IReadOnlyReactiveProperty<int> Power => _power;

        IntReactiveProperty _power = new IntReactiveProperty();

        public void SetPower(int power)
        {
            _power.Value = power;
        }
    }
}