using System.Collections;
using System.Collections.Generic;
using SoundManager;
using UnityEngine;
using Zenject;
using EnemyWeaponPool;

namespace EnemyWeaponInvoker
{
    public abstract class EnemyWeaponInvoker : MonoBehaviour, IEnemyWeaponInvoker
    {
        protected int _power;
        protected Transform _enemyTransform;
        protected IEnemyWeaponPool _enemyWeaponPool;
        protected ISoundManager _soundManager;

        public int Power => _power;

        public abstract EnemyWeaponType Type { get; }

        [Inject]
        public void Construct(
            IEnemyWeaponPool enemyWeaponPool,
            ISoundManager soundManager
        )
        {
            _enemyWeaponPool = enemyWeaponPool;
            _soundManager = soundManager;
        }

        #region//abstractMethod
        public abstract void Invoke();
        #endregion

        public void SetEnemyTransform(Transform enemyTransform)
        {
            _enemyTransform = enemyTransform;
        }

        public void SetPower(int power)
        {
            _power = power;
        }
    }
}