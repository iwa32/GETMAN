using GlobalInterface;

namespace NormalPlayerWeapon
{
    /// <summary>
    /// プレイヤーの武器
    /// </summary>
    public interface IPlayerWeapon: IEnemyAttacker
    {
        /// <summary>
        /// 武器を使用する
        /// </summary>
        void Use();
    }
}
