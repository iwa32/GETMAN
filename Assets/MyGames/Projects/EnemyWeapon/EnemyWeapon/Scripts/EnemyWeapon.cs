using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoundManager;
using Zenject;

namespace EnemyWeapon
{
    public abstract class EnemyWeapon : MonoBehaviour, IEnemyWeapon
    {
        protected int _power;
        protected Rigidbody _rigidbody;
        protected Transform _enemyTransform;
        protected ISoundManager _soundManager;

        public int Power => _power;

        public abstract EnemyWeaponType Type { get; }

        [Inject]
        public void Construct(
            ISoundManager soundManager
        )
        {
            _soundManager = soundManager;
        }

        public void SetEnemyTransform(Transform enemyTransform)
        {
            _enemyTransform = enemyTransform;
        }

        public void SetPower(int power)
        {
            _power = power;
        }

        public abstract void Use();
    }
}
