using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalInterface;

namespace EnemyWeapon
{
    public interface IEnemyWeapon: IPlayerAttacker
    {
        /// <summary>
        /// 武器を使用する
        /// </summary>
        void Use();

        /// <summary>
        /// エネミーのTransform
        /// </summary>
        /// <param name="playerTransform"></param>
        void SetEnemyTransform(Transform enemyTransform);
    }
}
