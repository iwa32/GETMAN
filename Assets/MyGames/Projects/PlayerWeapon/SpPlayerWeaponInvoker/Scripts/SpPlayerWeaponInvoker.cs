using System.Collections;
using System.Collections.Generic;
using ObjectPool;
using SoundManager;
using UnityEngine;
using Zenject;

namespace SpPlayerWeaponInvoker
{
    public abstract class SpPlayerWeaponInvoker : MonoBehaviour, ISpPlayerWeaponInvoker
    {
        protected int _power;
        protected Transform _playerTransform;
        protected ISpPlayerWeaponPool _spWeaponPool;//オブジェクトプール
        protected ISoundManager _soundManager;

        public int Power => _power;
        public abstract SpWeaponType Type { get; }


        #region//abstractMethod
        public abstract void Invoke();
        #endregion

        [Inject]
        public void Construct(
            ISpPlayerWeaponPool spWeaponPool,
            ISoundManager soundManager
        )
        {
            _spWeaponPool = spWeaponPool;
            _soundManager = soundManager;
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