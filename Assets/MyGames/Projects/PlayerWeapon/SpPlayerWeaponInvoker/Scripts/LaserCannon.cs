using System.Collections;
using System.Collections.Generic;
using SPW = SpPlayerWeapon;
using UnityEngine;

namespace SpPlayerWeaponInvoker
{
    public class LaserCannon : SpPlayerWeaponInvoker
    {
        [SerializeField]
        [Header("使用するレーザー武器のプレハブを設定")]
        SPW.PenetrateLaserByPlayer _laserPrefab;

        [SerializeField]
        [Header("プールに保持するレーザーの生成数を設定")]
        int _maxObjectCount = 3;

        SpWeaponType _type = SpWeaponType.LASER;
        public override SpWeaponType Type => _type;

        private void Awake()
        {
            _spWeaponPool.CreatePool(_laserPrefab, _maxObjectCount);
        }

        public override void Invoke()
        {
            Vector3 shootPos = _playerTransform.position + _playerTransform.forward;
            shootPos.y = _playerTransform.up.y;

            SPW.SpPlayerWeapon laser = _spWeaponPool.GetPool(_type);

            if (laser == null) return;

            laser.transform.position = shootPos;
            laser.SetPower(_power);
            laser.SetPlayerTransform(_playerTransform);
            laser.Use();
        }
    }
}
