using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace PlayerModel
{
    public interface IWeaponModel
    {
        IReadOnlyReactiveProperty<int> Power { get; }

        /// <summary>
        /// 攻撃力を設定します
        /// </summary>
        /// <param name="power"></param>
        void SetPower(int power);
    }
}
