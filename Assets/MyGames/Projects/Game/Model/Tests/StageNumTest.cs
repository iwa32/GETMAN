using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace GameModel
{
    public class StageNumTest
    {
        StageNumModel _stageNumModel;
        int _initialStageNum = 1;

        [SetUp]
        public void SetUp()
        {
            _stageNumModel = new StageNumModel();
            _stageNumModel.SetStageNum(_initialStageNum);
        }

        [Test, Description("ステージ番号が正しくセットされているか")]
        public void SetStageNumTest([Values(1, -1, 2)] int expected)
        {
            _stageNumModel.SetStageNum(expected);

            Assert.That(_stageNumModel.StageNum.Value, Is.EqualTo(expected));
        }

        [Test, Description("ステージ番号が加算されているか")]
        [TestCase(1, 2)]
        [TestCase(-1, 0)]
        [TestCase(0, 1)]
        public void IncreaseStageNumTest(int input, int expected)
        {
            _stageNumModel.SetStageNum(input);
            _stageNumModel.IncreaseStageNum();

            Assert.That(_stageNumModel.StageNum.Value, Is.EqualTo(expected));
        }
    }
}
