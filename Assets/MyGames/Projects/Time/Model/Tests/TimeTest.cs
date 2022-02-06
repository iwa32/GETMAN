using NUnit.Framework;

namespace TimeModel
{
    public class TimeTest
    {
        TimeModel _timeModel;

        [SetUp]
        public void SetUp()
        {
            _timeModel = new TimeModel();
        }

        [Test, Description("Timeが正しく設定されているか")]
        public void SetTimeTest([Values(0, -1, 1, -9999, 9999)] int expected)
        {
            _timeModel.SetTime(expected);
            Assert.That(_timeModel.Time.Value, Is.EqualTo(expected));
        }
    }
}