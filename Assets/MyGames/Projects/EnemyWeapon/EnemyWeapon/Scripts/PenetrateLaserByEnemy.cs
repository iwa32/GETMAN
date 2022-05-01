using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeaponUtility;

namespace EnemyWeapon
{
    public class PenetrateLaserByEnemy : EnemyWeapon
    {
        LaserUtility _laserUtility;
        EnemyWeaponType _type = EnemyWeaponType.LASER;

        public override EnemyWeaponType Type => _type;

        void Awake()
        {
            _laserUtility = GetComponent<LaserUtility>();
        }

        public override void Use()
        {
            _laserUtility.Use(_enemyTransform);
        }
    }
}
