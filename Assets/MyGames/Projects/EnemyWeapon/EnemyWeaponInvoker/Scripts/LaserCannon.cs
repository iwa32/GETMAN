using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EW = EnemyWeapon;

namespace EnemyWeaponInvoker
{
    public class LaserCannon : EnemyWeaponInvoker
    {
        [SerializeField]
        [Header("使用するレーザー武器のプレハブを設定")]
        EW.Laser _laserPrefab;

        [SerializeField]
        [Header("プールに保持するレーザーの生成数を設定")]
        int _maxObjectCount = 3;

        [Header("レーザーの射出位置を設定")]
        float _shootingHeight = 1.5f;

        EnemyWeaponType _type = EnemyWeaponType.LASER;
        public override EnemyWeaponType Type => _type;

        private void Awake()
        {
            _enemyWeaponPool.CreatePool(_laserPrefab, _maxObjectCount);
        }

        public override void Invoke()
        {
            Vector3 shootPos = _enemyTransform.position + _enemyTransform.forward;
            shootPos.y = _shootingHeight;

            EW.EnemyWeapon laser = _enemyWeaponPool.GetPool(_type);

            if (laser == null) return;

            laser.transform.position = shootPos;
            laser.SetPower(_power);
            laser.SetEnemyTransform(_enemyTransform);
            laser.Use();
        }
    }
}