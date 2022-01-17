using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UniRx;
using UniRx.Triggers;

namespace EnemyView
{
    public class TrackingAreaView : MonoBehaviour
    {
        [SerializeField]
        [Header("追跡範囲の角度を設定")]
        float _trackingAngle;

        [SerializeField]
        [Header("追跡範囲のコライダーを設定")]
        SphereCollider _trackingAreaCollider;

        Vector3 _targetPlayerPosition;
        BoolReactiveProperty _canTrack = new BoolReactiveProperty();//追跡フラグ


        public IReactiveProperty<bool> CanTrack => _canTrack;
        public Vector3 TargetPlayerPosition => _targetPlayerPosition;


        void Start()
        {
            OnTrackingAreaStay()
                .Subscribe(collider => CheckPlayer(collider));

            OnTrackingAreaExit()
                .Subscribe(_ =>
                {
                    _canTrack.Value = false;
                });
        }

        /// <summary>
        /// プレイヤーの接触を確認します
        /// </summary>
        /// <param name="collider"></param>
        void CheckPlayer(Collider collider)
        {
            if (collider.CompareTag("Player") == false) return;

            //プレイヤーの方向から角度を取得します
            Vector3 playerDirection
                = collider.gameObject.transform.position - transform.position;
            float targetAngle = Vector3.Angle(transform.forward, playerDirection);

            if (targetAngle > _trackingAngle) return;//追跡範囲外なら追跡しない

            _targetPlayerPosition = collider.gameObject.transform.position;
            _canTrack.Value = true;
        }

        /// <summary>
        /// 追跡エリアに入っている間
        /// </summary>
        /// <returns></returns>
        IObservable<Collider> OnTrackingAreaStay()
        {
            return this.OnTriggerStayAsObservable();
        }

        /// <summary>
        /// 追跡エリアを出たか
        /// </summary>
        /// <returns></returns>
        IObservable<Collider> OnTrackingAreaExit()
        {
            return this.OnTriggerExitAsObservable();
        }

#if UNITY_EDITOR
        //　サーチする角度表示
        private void OnDrawGizmos()
        {
            Handles.color = Color.red;
            Handles.DrawSolidArc(transform.position, Vector3.up, Quaternion.Euler(0f, -_trackingAngle, 0f) * transform.forward, _trackingAngle * 2f, _trackingAreaCollider.radius);
        }
#endif
    }
}