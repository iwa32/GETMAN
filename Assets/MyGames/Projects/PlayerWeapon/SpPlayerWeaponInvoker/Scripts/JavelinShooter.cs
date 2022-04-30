using System.Collections;
using System.Collections.Generic;
using SPW = SpPlayerWeapon;
using UnityEngine;

namespace SpPlayerWeaponInvoker
{
    public class JavelinShooter : SpPlayerWeaponInvoker
    {
        [SerializeField]
        [Header("投槍のプレハブを設定")]
        SPW.Javelin _javelinPrefab;

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
            //Yの回転軸をプレイヤーに合わせる
            _prefabEulerAngles.y = _playerTransform.rotation.eulerAngles.y;

            //射出位置を設定　プレイヤーの前方へ
            Vector3 shootPos = _playerTransform.position + _playerTransform.forward;
            shootPos.y = _shootingHeight;

            //ジャベリンを取得
            SPW.SpPlayerWeapon javelin
                = _spWeaponPool.GetPool(_type);

            if (javelin == null) return;

            javelin.transform.position = shootPos;
            javelin.transform.rotation = Quaternion.Euler(_prefabEulerAngles);
            javelin.SetPower(_power);
            javelin.SetPlayerTransform(_playerTransform);
            javelin.Use();
            _soundManager.PlaySE(SEType.THROW);
        }
    }
}