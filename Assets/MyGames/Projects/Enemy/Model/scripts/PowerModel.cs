using UniRx;

namespace EnemyModel
{
    public class PowerModel : IPowerModel
    {
        IntReactiveProperty _power = new IntReactiveProperty();

        public IReadOnlyReactiveProperty<int> Power => _power;

        public void SetPower(int power)
        {
            _power.Value = power;
        }
    }
}
