namespace GlobalInterface
{
    /// <summary>
    /// プレイヤーへの攻撃者
    /// </summary>
    public interface IPlayerAttacker
    {
        int Power { get; }

        /// <summary>
        /// パワーの設定
        /// </summary>
        /// <param name="power"></param>
        void SetPower(int power);
    }
}