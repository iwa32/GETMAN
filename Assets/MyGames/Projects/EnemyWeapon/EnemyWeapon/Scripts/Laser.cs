using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Trigger;

namespace EnemyWeapon
{
    public class Laser : EnemyWeapon
    {
        [SerializeField]
        [Header("瞬時に放つ力を設定")]
        float _force = 50;

        [SerializeField]
        [Header("レーザーの軌跡を設定")]
        TrailRenderer _trailRenderer;

        EnemyWeaponType _type = EnemyWeaponType.LASER;
        ObservableTrigger _trigger;

        public override EnemyWeaponType Type => _type;

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
            //壁に接触で削除
            if (collider.CompareTag("Wall"))
            {
                gameObject.SetActive(false);
                _trailRenderer.Clear();
            }
        }

        public override void Use()
        {
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.AddForce(
                _enemyTransform.forward * _force,
                ForceMode.VelocityChange
                );
        }
    }
}
