using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UniRx;

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

        Vector3 _targetPosition;
        BoolReactiveProperty _canTrack = new BoolReactiveProperty();//追跡フラグ
        TriggerView.TriggerView _triggerView;


        public IReactiveProperty<bool> CanTrack => _canTrack;
        public Vector3 TargetPosition => _targetPosition;

        void Awake()
        {
            _triggerView = GetComponent<TriggerView.TriggerView>();
        }

        void Start()
        {
            _triggerView.OnTriggerStay()
                .Subscribe(collider => CheckTarget(collider));

            _triggerView.OnTriggerExit()
                .Subscribe(_ =>
                {
                    _canTrack.Value = false;
                });
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

            _targetPosition = collider.gameObject.transform.position;
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