using UniRx;

namespace PlayerModel
{
    public interface IHpModel
    {
        IReadOnlyReactiveProperty<int> Hp { get; }

        /// <summary>
        /// Hpの増加
        /// </summary>
        /// <param name="hp"></param>
        void AddHp(int hp);

        /// <summary>
        /// Hpの減少
        /// </summary>
        /// <param name="hp"></param>
        void ReduceHp(int hp);

        /// <summary>
        /// Hpのリセット
        /// </summary>
        void ResetHp();
    }
}