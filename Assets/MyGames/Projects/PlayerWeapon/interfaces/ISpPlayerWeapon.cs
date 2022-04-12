using UnityEngine;

namespace PlayerWeapon
{
    /// <summary>
    /// プレイヤーのSP武器
    /// </summary>
    public interface ISpPlayerWeapon : IPlayerWeapon
    {
        /// <summary>
        /// 武器の種類
        /// </summary>
        SpWeaponType Type { get; }

        /// <summary>
        /// プレイヤーのTransform
        /// </summary>
        /// <param name="playerTransform"></param>
        void SetPlayerTransform(Transform playerTransform);
    }
}
