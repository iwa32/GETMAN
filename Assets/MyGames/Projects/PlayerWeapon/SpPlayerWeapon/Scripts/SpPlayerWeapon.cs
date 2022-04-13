using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpPlayerWeapon
{
    public abstract class SpPlayerWeapon : MonoBehaviour, ISpPlayerWeapon
    {
        protected int _power;
        protected Rigidbody _rigidbody;
        protected Transform _playerTransform;
        protected SpWeaponType _type = SpWeaponType.NONE;

        public int Power => _power;
        public SpWeaponType Type => _type;


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
