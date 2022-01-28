using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using TriggerView;

namespace EnemyView
{
    public class ForwardObstacleCheckView : MonoBehaviour
    {
        CollisionView _collisionView;
        TriggerView.TriggerView _triggerView;
        BoolReactiveProperty _isOn = new BoolReactiveProperty();

        public IReactiveProperty<bool> IsOn => _isOn;

        void Awake()
        {
            _collisionView = GetComponent<CollisionView>();
            _triggerView = GetComponent<TriggerView.TriggerView>();
        }

        void Start()
        {
            Bind();
        }

        void Bind()
        {
            //障害物の接触を確認
            _collisionView
                .OnCollisionStay()
                .ThrottleFirst(TimeSpan.FromMilliseconds(1000))
                .Subscribe(collision => CheckObstacle(collision.collider))
                .AddTo(this);

            _triggerView
                .OnTriggerStay()
                .ThrottleFirst(TimeSpan.FromMilliseconds(1000))
                .Subscribe(collider => CheckObstacle(collider))
                .AddTo(this);

            //障害物との接触が離れたことを確認
            _collisionView
                .OnCollisionExit()
                .ThrottleFirst(TimeSpan.FromMilliseconds(1000))
                .Subscribe(_ => _isOn.Value = false)
                .AddTo(this);

            _triggerView
                .OnTriggerExit()
                .ThrottleFirst(TimeSpan.FromMilliseconds(1000))
                .Subscribe(_ => _isOn.Value = false)
                .AddTo(this);
        }

        /// <summary>
        /// 障害物を確認します
        /// </summary>
        void CheckObstacle(Collider collider)
        {
            if (collider.CompareTag("Obstacle"))
            {
                _isOn.Value = true;
            }
        }
    }
}