using System.Collections;
using System.Collections.Generic;
using SPW = SpPlayerWeapon;
using UnityEngine;
using WeaponInvokerUtility;

namespace SpPlayerWeaponInvoker
{
    public class LaserCannon : SpPlayerWeaponInvoker
    {
        [SerializeField]
        [Header("使用するレーザー武器のプレハブを設定")]
        SPW.PenetrateLaserByPlayer _laserPrefab;

        LaserCannonUtility _laserCannonUtility;
        SpWeaponType _type = SpWeaponType.LASER;
        public override SpWeaponType Type => _type;

        private void Awake()
        {
            _laserCannonUtility = GetComponent<LaserCannonUtility>();
            _spWeaponPool.CreatePool(_laserPrefab, _laserCannonUtility.MaxObjectCount);
        }

        public override void Invoke()
        {
            SPW.SpPlayerWeapon laser = _spWeaponPool.GetPool(_type);

            if (laser == null) return;

            laser.transform.position = _laserCannonUtility.GetShootPosition(_playerTransform);
            laser.SetPower(_power);
            laser.SetPlayerTransform(_playerTransform);
            laser.Use();
        }
    }
}
