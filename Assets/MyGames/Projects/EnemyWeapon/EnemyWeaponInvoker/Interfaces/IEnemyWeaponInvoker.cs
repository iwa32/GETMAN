using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemyWeaponInvoker
{
    public interface IEnemyWeaponInvoker
    {
        int Power { get; }

        /// <summary>
        /// 武器の種類
        /// </summary>
        EnemyWeaponType Type { get; }

        /// <summary>
        /// エネミーのTransform
        /// </summary>
        /// <param name="enemyTransform"></param>
        void SetEnemyTransform(Transform enemyTransform);

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
