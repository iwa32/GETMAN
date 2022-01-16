using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace GameModel
{
    public class PointTest
    {
        PointModel _pointModel;
        int _initialPoint = 0;

        [SetUp]
        public void SetUp()
        {
            _pointModel = new PointModel();
            _pointModel.SetPoint(_initialPoint);
        }

        [Test, Description("ポイントが正しくセットされているか")]
        public void SetPointTest([Values(1, -1, 2)] int expected)
        {
            _pointModel.SetPoint(expected);

            Assert.That(_pointModel.Point.Value, Is.EqualTo(expected));
        }

        [Test, Description("ポイントが加算されているか")]
        [TestCase(1, 1)]
        [TestCase(0, 0)]
        [TestCase(-1, -1)]
        public void AddPointTest(int input, int expected)
        {
            _pointModel.AddPoint(input);
            Assert.That(_pointModel.Point.Value, Is.EqualTo(expected));
        }
    }
}
