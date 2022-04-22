using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Trigger;

namespace SpPlayerWeapon
{
    public class Javelin : SpPlayerWeapon
    {
        [SerializeField]
        [Header("射出する力を設定")]
        float _force = 5;

        SpWeaponType _type = SpWeaponType.JAVELIN;
        ObservableTrigger _trigger;

        public override SpWeaponType Type => _type;

        void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _trigger = GetComponent<ObservableTrigger>();
            Bind();
        }

        void Bind()
        {
            _trigger.OnTriggerEnter()
                .Subscribe(collider => Hit(collider))
                .AddTo(this);
        }

        void Hit(Collider collider)
        {
            if(collider.CompareTag("Enemy"))
            {
                _soundManager.PlaySE(SEType.SWORD_HITTED);
            }
            
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 射出します
        /// </summary>
        public override void Use()
        {
            //弾を使いまわしているため、一度力をリセットします
            _rigidbody.velocity = Vector3.zero;

            _rigidbody.AddForce((_playerTransform.forward + _playerTransform.up) * _force, ForceMode.Impulse);
        }
    }
}
