using System.Collections;
using System.Collections.Generic;
using SpPlayerWeapon;
using UnityEngine;
using Zenject;
using ObjectPool;

namespace SpPlayerWeaponInvoker
{
    public class JavelinShooter : MonoBehaviour, ISpPlayerWeaponInvoker
    {
        [SerializeField]
        [Header("投槍のプレハブを設定")]
        Javelin _javelinPrefab;

        [SerializeField]
        [Header("射出する高さを設定")]
        float _shootingHeight = 1;

        [SerializeField]
        [Header("一度の連射数を設定")]
        int _shootingCount = 3;

        int _power = 1;
        SpWeaponType _type = SpWeaponType.JAVELIN;
        Transform _playerTransform;
        ISpPlayerWeaponPool _spWeaponPool;

        public int Power => _power;
        public SpWeaponType Type => _type;


        //[Inject]
        //public void Construct(ISpPlayerWeaponPool spWeaponPool)
        //{
        //    _spWeaponPool = spWeaponPool;
        //}

        public void SetPlayerTransform(Transform playerTransform)
        {
            _playerTransform = playerTransform;
        }

        public void SetPower(int power)
        {
            _power = power;
        }

        /// <summary>
        /// 武器を使用します
        /// </summary>
        public void Invoke()
        {
            //todo diの方法を後で考える
            if (_spWeaponPool == null)
                _spWeaponPool = new SpWeaponPool();

            //プール作成
            if (_spWeaponPool.SpWeaponList.Count == 0)
            {
                _spWeaponPool.CreatePool(_javelinPrefab, _shootingCount);
            }

            //Yの回転軸をプレイヤーに合わせる
            Vector3 eulerAngles = _javelinPrefab.transform.rotation.eulerAngles;
            eulerAngles.y = _playerTransform.rotation.eulerAngles.y;

            //射出位置を設定
            Vector3 shootPos = _playerTransform.position;
            shootPos.y = _shootingHeight;

            //ジャベリンを取得
            ISpPlayerWeapon javelin
                = _spWeaponPool.GetPool(shootPos,Quaternion.Euler(eulerAngles));

            if (javelin == null) return;

            javelin.SetPower(_power);
            javelin.SetPlayerTransform(_playerTransform);
            javelin.Use();
        }
    }
}