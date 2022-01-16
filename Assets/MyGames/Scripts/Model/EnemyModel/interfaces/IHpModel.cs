using UniRx;

namespace EnemyModel
{
    public interface IHpModel
    {
        IReadOnlyReactiveProperty<int> Hp { get; }

        void SetHp(int hp);

        /// <summary>
        /// HPを減らします
        /// </summary>
        /// <param name="hp"></param>
        void ReduceHp(int hp);
    }
}
