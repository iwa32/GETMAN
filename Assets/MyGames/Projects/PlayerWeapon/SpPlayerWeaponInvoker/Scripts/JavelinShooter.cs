using System.Collections;
using System.Collections.Generic;
using SpPlayerWeapon;
using UnityEngine;
using Zenject;
using ObjectPool;

namespace SpPlayerWeaponInvoker
{
    public class JavelinShooter : SpPlayerWeaponInvoker
    {
        [SerializeField]
        [Header("投槍のプレハブを設定")]
        Javelin _javelinPrefab;

        [SerializeField]
        [Header("射出する高さを設定")]
        float _shootingHeight = 1;

        [SerializeField]
        [Header("一度の画面表示数")]
        int _shootingCount = 3;

        SpWeaponType _type = SpWeaponType.JAVELIN;
        Vector3 _prefabEulerAngles;

        public override SpWeaponType Type => _type;

        private void Awake()
        {
            _spWeaponPool.CreatePool(_javelinPrefab, _shootingCount);
            _prefabEulerAngles = _javelinPrefab.transform.rotation.eulerAngles;
        }

        /// <summary>
        /// 武器を使用します
        /// </summary>
        public override void Invoke()
        {
            if (_spWeaponPool.SpWeaponList.Count == 0) return;

            //Yの回転軸をプレイヤーに合わせる
            _prefabEulerAngles.y = _playerTransform.rotation.eulerAngles.y;

            //射出位置を設定
            Vector3 shootPos = _playerTransform.position;
            shootPos.y = _shootingHeight;

            //ジャベリンを取得
            ISpPlayerWeapon javelin
                = _spWeaponPool.GetPool(shootPos,Quaternion.Euler(_prefabEulerAngles));

            if (javelin == null) return;

            javelin.SetPower(_power);
            javelin.SetPlayerTransform(_playerTransform);
            javelin.Use();
        }
    }
}