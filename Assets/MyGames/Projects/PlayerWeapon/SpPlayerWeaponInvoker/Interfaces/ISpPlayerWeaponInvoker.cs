using UnityEngine;

namespace SpPlayerWeaponInvoker
{
    /// <summary>
    /// プレイヤーのSP武器を実行します
    /// </summary>
    public interface ISpPlayerWeaponInvoker
    {
        /// <summary>
        /// 武器の種類
        /// </summary>
        SpWeaponType Type { get; }

        int Power { get; }

        /// <summary>
        /// プレイヤーのTransform
        /// </summary>
        /// <param name="playerTransform"></param>
        void SetPlayerTransform(Transform playerTransform);

        /// <summary>
        /// 実行
        /// </summary>
        void Invoke();

        /// <summary>
        /// パワーの設定
        /// </summary>
        /// <param name="power"></param>
        void SetPower(int power);
    }
}
