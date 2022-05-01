using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Trigger;
using Zenject;
using SoundManager;
using UniRx;

namespace WeaponUtility
{
    public class LaserUtility : MonoBehaviour
    {
        [SerializeField]
        [Header("瞬時に放つ力を設定")]
        float _force = 50;

        [SerializeField]
        [Header("レーザーの軌跡を設定")]
        TrailRenderer _trailRenderer;

        Rigidbody _rigidbody;
        ObservableTrigger _trigger;
        ISoundManager _soundManager;
        Transform _shooterTransform;

        [Inject]
        public void Construct(
            ISoundManager soundManager
        )
        {
            _soundManager = soundManager;
        }

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

        public void Use(Transform shooter)
        {
            //一度だけ設定
            if (_shooterTransform == null && shooter != null)
                _shooterTransform = shooter;

            _rigidbody.velocity = Vector3.zero;
            _rigidbody.AddForce(
                _shooterTransform.forward * _force,
                ForceMode.VelocityChange
                );
        }
    }
}