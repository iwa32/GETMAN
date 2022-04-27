using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UniRx;
using Trigger;

namespace TrackingAreaView
{
    public class TrackingAreaView : MonoBehaviour
    {
        [SerializeField]
        [Header("追跡範囲の角度を設定")]
        float _trackingAngle;

        [SerializeField]
        [Header("追跡範囲のコライダーを設定")]
        SphereCollider _trackingAreaCollider;

        [SerializeField]
        [Header("追跡対象のタグ名を設定")]
        string _targetTagName;

        Transform _targetTransform;
        BoolReactiveProperty _canTrack = new BoolReactiveProperty();//追跡フラグ
        ObservableTrigger _trigger;


        public IReactiveProperty<bool> CanTrack => _canTrack;
        public Transform TargetTransform => _targetTransform;

        void Awake()
        {
            _trigger = GetComponent<ObservableTrigger>();
        }

        void Start()
        {
            _trigger.OnTriggerStay()
                .Subscribe(collider => CheckTarget(collider))
                .AddTo(this);

            _trigger.OnTriggerExit()
                .Subscribe(_ => _canTrack.Value = false)
                .AddTo(this);
        }

        void OnDisable()
        {
            //フラグをリセット
            _canTrack.Value = false;
        }

        /// <summary>
        /// ターゲットの接触を確認します
        /// </summary>
        /// <param name="collider"></param>
        void CheckTarget(Collider collider)
        {
            if (collider.CompareTag(_targetTagName) == false) return;
            //ターゲットの方向から角度を取得します
            Vector3 targetDirection
                = collider.gameObject.transform.position - transform.position;
            float targetAngle = Vector3.Angle(transform.forward, targetDirection);

            if (targetAngle > _trackingAngle) return;//追跡範囲外なら追跡しない

            _targetTransform = collider.gameObject.transform;
            _canTrack.Value = true;
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