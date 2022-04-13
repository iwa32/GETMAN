using UnityEngine;
using GlobalInterface;

namespace SpPlayerWeapon
{
    /// <summary>
    /// プレイヤーのSP武器
    /// </summary>
    public interface ISpPlayerWeapon : IEnemyAttacker
    {
        /// <summary>
        /// 武器の種類
        /// </summary>
        SpWeaponType Type { get; }

        /// <summary>
        /// 武器を使用する
        /// </summary>
        void Use();

        /// <summary>
        /// プレイヤーのTransform
        /// </summary>
        /// <param name="playerTransform"></param>
        void SetPlayerTransform(Transform playerTransform);
    }
}
