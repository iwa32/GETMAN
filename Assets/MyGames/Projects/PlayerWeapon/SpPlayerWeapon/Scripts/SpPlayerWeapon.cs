using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoundManager;
using Zenject;

namespace SpPlayerWeapon
{
    public abstract class SpPlayerWeapon : MonoBehaviour, ISpPlayerWeapon
    {
        protected int _power;
        protected Rigidbody _rigidbody;
        protected Transform _playerTransform;
        protected ISoundManager _soundManager;

        public int Power => _power;
        public abstract SpWeaponType Type { get; }


        [Inject]
        public void Construct(
            ISoundManager soundManager
        )
        {
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

        public abstract void Use();
    }
}
