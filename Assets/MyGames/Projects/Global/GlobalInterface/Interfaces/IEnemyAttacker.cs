namespace GlobalInterface
{
    /// <summary>
    /// エネミーへの攻撃者
    /// </summary>
    public interface IEnemyAttacker
    {
        int Power { get; }

        /// <summary>
        /// パワーの設定
        /// </summary>
        /// <param name="power"></param>
        void SetPower(int power);
    }
}