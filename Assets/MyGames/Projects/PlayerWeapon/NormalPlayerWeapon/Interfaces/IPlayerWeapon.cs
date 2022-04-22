using GlobalInterface;

namespace NormalPlayerWeapon
{
    /// <summary>
    /// プレイヤーの武器
    /// </summary>
    public interface IPlayerWeapon: IEnemyAttacker
    {
        /// <summary>
        /// 武器の使用を開始する
        /// </summary>
        void StartMotion();

        /// <summary>
        /// 武器の使用を止める
        /// </summary>
        void EndMotion();
    }
}
