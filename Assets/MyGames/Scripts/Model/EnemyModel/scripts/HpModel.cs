using UniRx;

namespace EnemyModel
{
    public class HpModel : IHpModel
    {
        IntReactiveProperty _hp = new IntReactiveProperty();

        public IReadOnlyReactiveProperty<int> Hp => _hp;

        public void SetHp(int hp)
        {
            _hp.Value = hp;
        }

        public void ReduceHp(int hp)
        {
            _hp.Value -= hp;
        }
    }
}
