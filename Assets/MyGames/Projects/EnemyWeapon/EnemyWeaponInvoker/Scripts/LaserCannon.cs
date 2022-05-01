using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EW = EnemyWeapon;
using WeaponInvokerUtility;

namespace EnemyWeaponInvoker
{
    public class LaserCannon : EnemyWeaponInvoker
    {
        [SerializeField]
        [Header("使用するレーザー武器のプレハブを設定")]
        EW.PenetrateLaserByEnemy _laserPrefab;

        LaserCannonUtility _laserCannonUtility;
        EnemyWeaponType _type = EnemyWeaponType.LASER;
        public override EnemyWeaponType Type => _type;

        private void Awake()
        {
            _laserCannonUtility = GetComponent<LaserCannonUtility>();
            _enemyWeaponPool.CreatePool(_laserPrefab, _laserCannonUtility.MaxObjectCount);
        }

        public override void Invoke()
        {
            EW.EnemyWeapon laser = _enemyWeaponPool.GetPool(_type);

            if (laser == null) return;

            laser.transform.position = _laserCannonUtility.GetShootPosition(_enemyTransform);
            laser.SetPower(_power);
            laser.SetEnemyTransform(_enemyTransform);
            laser.Use();
        }
    }
}