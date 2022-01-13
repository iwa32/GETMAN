using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace PlayerModel
{
    public class HpModel : IHpModel
    {
        public IReadOnlyReactiveProperty<int> Hp => _hp;

        IntReactiveProperty _hp = new IntReactiveProperty();

        public void AddHp(int hp)
        {
            _hp.Value += hp;
        }

        public void ReduceHp(int hp)
        {
            _hp.Value -= hp;
        }

        public void SetHp(int hp)
        {
            _hp.Value = hp;
        }
    }
}
