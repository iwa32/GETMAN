using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Collision;
using Trigger;

namespace EnemyView
{
    public class ForwardObstacleCheckView : MonoBehaviour
    {
        ObservableCollision _collision;
        ObservableTrigger _trigger;
        BoolReactiveProperty _isOn = new BoolReactiveProperty();

        public IReactiveProperty<bool> IsOn => _isOn;

        void Awake()
        {
            _collision = GetComponent<ObservableCollision>();
            _trigger = GetComponent<ObservableTrigger>();
        }

        void Start()
        {
            Bind();
        }

        void Bind()
        {
            //障害物の接触を確認
            _collision
                .OnCollisionStay()
                .ThrottleFirst(TimeSpan.FromMilliseconds(1000))
                .Subscribe(collision => CheckObstacle(collision.collider))
                .AddTo(this);

            _trigger
                .OnTriggerStay()
                .ThrottleFirst(TimeSpan.FromMilliseconds(1000))
                .Subscribe(collider => CheckObstacle(collider))
                .AddTo(this);

            //障害物との接触が離れたことを確認
            _collision
                .OnCollisionExit()
                .ThrottleFirst(TimeSpan.FromMilliseconds(1000))
                .Subscribe(_ => _isOn.Value = false)
                .AddTo(this);

            _trigger
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

        /// <summary>
        /// 障害物判定のフラグを設定
        /// </summary>
        /// <param name="isOn"></param>
        public void SetIsOn(bool isOn)
        {
            _isOn.Value = isOn;
        }
    }
}