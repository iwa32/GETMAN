using UniRx;

namespace EnemyModel
{
    public interface IPowerModel
    {
        IReadOnlyReactiveProperty<int> Power { get; }

        void SetPower(int power);
    }
}
