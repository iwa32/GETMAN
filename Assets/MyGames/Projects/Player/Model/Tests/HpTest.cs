using NUnit.Framework;

namespace PlayerModel
{
    public class HpTest
    {
        HpModel _hpModel;
        int _initialHp = 3;

        [SetUp]
        public void SetUp()
        {
            _hpModel = new HpModel();
            _hpModel.SetHp(_initialHp);
        }

        [Test, Description("Hpが正しくセットされているか")]
        public void SetHpTest([Values(1, -1, 2)] int expected)
        {
            _hpModel.SetHp(expected);

            Assert.That(_hpModel.Hp.Value, Is.EqualTo(expected));
        }

        [Test, Description("Hpが増加しているか")]
        [TestCase(1, 4)]
        [TestCase(0, 3)]
        [TestCase(-1, 2)]
        public void AddHpTest(int input, int expected)
        {
            _hpModel.AddHp(input);

            Assert.That(_hpModel.Hp.Value, Is.EqualTo(expected));
        }

        [Test, Description("Hpが減少しているか")]
        [TestCase(1, 2)]
        [TestCase(4, -1)]
        [TestCase(-3, 6)]
        public void ReduceHpTest(int input, int expected)
        {
            _hpModel.ReduceHp(input);

            Assert.That(_hpModel.Hp.Value, Is.EqualTo(expected));
        }
    }
}