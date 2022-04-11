namespace PlayerWeapon
{
    /// <summary>
    /// プレイヤーの武器
    /// </summary>
    public interface IPlayerWeapon
    {
        int Power { get; }

        /// <summary>
        /// パワーの設定
        /// </summary>
        /// <param name="power"></param>
        void SetPower(int power);

        /// <summary>
        /// 武器を使用する
        /// </summary>
        void Use();
    }
}
