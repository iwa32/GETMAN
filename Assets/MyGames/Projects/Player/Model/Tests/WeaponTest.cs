using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PlayerModel
{
    public class WeaponTest
    {
        WeaponModel _weaponModel;
        int _initialPower = 1;

        [SetUp]
        public void SetUp()
        {
            _weaponModel = new WeaponModel();
            _weaponModel.SetPower(_initialPower);
        }

        [Test, Description("攻撃力が正しく設定されているか")]
        public void SetPowerTest([Values(-1, 0, 1)] int power)
        {
            _weaponModel.SetPower(power);

            Assert.That(_weaponModel.Power.Value, Is.EqualTo(power));
        }
    }
}
