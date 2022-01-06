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
        int initialHp;

        internal HpModel(int hp)
        {
            _hp.Value = hp;
            initialHp = hp;
        }

        public void AddHp(int hp)
        {
            _hp.Value += hp;
        }

        public void ReduceHp(int hp)
        {
            _hp.Value -= hp;
        }

        public void ResetHp()
        {
            _hp.Value = initialHp;
        }
    }
}
