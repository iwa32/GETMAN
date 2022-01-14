using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using static StateType;

namespace PlayerModel
{
    public class StateTest
    {
        StateModel _stateModel;

        [SetUp]
        public void SetUp()
        {
            _stateModel = new StateModel();
            _stateModel.SetState(WAIT);
        }

        [Test, Description("プレイヤーの状態が正しく設定されているか")]
        public void SetStateTest([Values] StateType state)
        {
            _stateModel.SetState(state);

            Assert.That(_stateModel.State.Value, Is.EqualTo(state));
        }
    }
}
