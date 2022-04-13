using System.Collections;
using System.Collections.Generic;
using ObjectPool;
using UnityEngine;
using Zenject;

namespace SpPlayerWeaponInvoker
{
    public abstract class SpPlayerWeaponInvoker : MonoBehaviour, ISpPlayerWeaponInvoker
    {
        protected int _power;
        protected Transform _playerTransform;
        protected ISpPlayerWeaponPool _spWeaponPool;//オブジェクトプール

        public int Power => _power;
        public abstract SpWeaponType Type { get; }


        #region//abstractMethod
        public abstract void Invoke();
        #endregion

        [Inject]
        public void Construct(ISpPlayerWeaponPool spWeaponPool)
        {
            _spWeaponPool = spWeaponPool;
        }

        public void SetPlayerTransform(Transform playerTransform)
        {
            _playerTransform = playerTransform;
        }

        public void SetPower(int power)
        {
            _power = power;
        }
    }
}