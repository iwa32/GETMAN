using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace EnemyModel
{
    public class PowerTest
    {
        PowerModel _powerModel;
        int _initialPower = 1;

        [SetUp]
        public void SetUp()
        {
            _powerModel = new PowerModel();
            _powerModel.SetPower(_initialPower);
        }

        [Test, Description("パワーが正しくセットされているか")]
        public void SetPowerTest([Values(1, -1, 2)] int expected)
        {
            _powerModel.SetPower(expected);

            Assert.That(_powerModel.Power.Value, Is.EqualTo(expected));
        }
    }
}
