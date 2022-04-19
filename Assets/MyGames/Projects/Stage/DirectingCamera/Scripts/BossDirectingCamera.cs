using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

namespace StageDirectingCamera
{
    /// <summary>
    /// ボス演出用カメラ
    /// </summary>
    public class BossDirectingCamera : MonoBehaviour, IBossDirectingCamera
    {
        readonly int EndDelay = 1;//演出終了後に遅らせる時間

        [SerializeField]
        [Header("ボスを撮る距離を設定します※正数で指定")]
        [Range(0, 10)]
        float _cameraDistance = 5f;

        [SerializeField]
        [Header("カメラの高さを設定します")]
        float _cameraHeight = 10f;

        [SerializeField]
        [Header("一回の演出終了時間")]
        float _directingTime = 3;

        Transform _bossTransform;
        Vector3 _startPosition;
        Vector3 _endPosition;
        Camera _directingCamera;
        CancellationTokenSource _cts = new CancellationTokenSource();

        void Awake()
        {
            _directingCamera = GetComponent<Camera>();
        }

        void Start()
        {
            Initialize();
        }

        void Initialize()
        {
            _directingCamera.enabled = false;
        }

        public async UniTask Direct(Transform targetBoss)
        {
            CancellationToken token = _cts.Token;

            SetUpDirecting(targetBoss);
            await GoAroundRightTheBoss(token);
            await GoAroundLeftTheBoss(token);
            await ZoomOutTheBoss(token);
            Initialize();
        }

        void SetUpDirecting(Transform targetBoss)
        {
            _bossTransform = targetBoss;
            _directingCamera.enabled = true;
        }

        /// <summary>
        /// ボスの右を周ります
        /// </summary>
        /// <returns></returns>
        async UniTask GoAroundRightTheBoss(CancellationToken token)
        {
            //ボスの右斜め前に配置します
            Vector3 offset = (_bossTransform.transform.right + _bossTransform.transform.forward).normalized;
            GetReadyToGoAroundTheBoss(offset);
            await GoAroundTheBoss(token);
        }

        /// <summary>
        /// ボスの左を周ります
        /// </summary>
        /// <returns></returns>
        async UniTask GoAroundLeftTheBoss(CancellationToken token)
        {
            //ボスの左斜め前に配置します
            Vector3 offset = (-_bossTransform.transform.right + _bossTransform.transform.forward).normalized;
            GetReadyToGoAroundTheBoss(offset);
            await GoAroundTheBoss(token);
        }

        /// <summary>
        /// ズームアウトします
        /// </summary>
        /// <returns></returns>
        async UniTask ZoomOutTheBoss(CancellationToken token)
        {
            //カメラを敵の斜め上に設定
            Vector3 cameraDistance = (_bossTransform.forward + _bossTransform.up).normalized * _cameraDistance;
            _directingCamera.transform.rotation = Quaternion.identity;
            _directingCamera.transform.position = cameraDistance;

            //演出中後ろに下がっていきます
            float elapsedTime = 0;
            while (elapsedTime < _directingTime)
            {
                _directingCamera.transform.position += -_directingCamera.transform.forward * Time.deltaTime;

                elapsedTime += Time.deltaTime;
                await UniTask.Yield(token);
            }

            //演出終了後少し遅らせます
            await UniTask.Delay(TimeSpan.FromSeconds(EndDelay), cancellationToken: token);
        }

        /// <summary>
        /// ボスを周る準備をします
        /// </summary>
        /// <param name="referencePosition"></param>
        void GetReadyToGoAroundTheBoss(Vector3 referencePosition)
        {
            //カメラを基準位置に配置
            _directingCamera.transform.position
                = _bossTransform.transform.position + referencePosition;

            //カメラを設定し、ボスを撮ります
            _directingCamera.transform.LookAt(_bossTransform);

            //カメラの距離を設定します
            Vector3 cameraDistance = _directingCamera.transform.forward * -_cameraDistance;
            cameraDistance.y = _cameraHeight;
            _directingCamera.transform.position = _bossTransform.position + cameraDistance;

            _directingCamera.transform.LookAt(_bossTransform);

            _startPosition = _directingCamera.transform.position;
            _endPosition = _bossTransform.position + new Vector3(cameraDistance.x, cameraDistance.y, -cameraDistance.z);
        }

        /// <summary>
        /// ボスを周ります
        /// </summary>
        /// <returns></returns>
        async UniTask GoAroundTheBoss(CancellationToken token)
        {
            float distance = Vector3.Distance(_startPosition, _endPosition);
            while (distance > 0.1f)
            {
                float time = (Time.deltaTime * _directingTime) / distance;

                //ボスを撮りながら円運動する
                _directingCamera.transform.position
                    = Vector3.Slerp(_directingCamera.transform.position, _endPosition, time);
                _directingCamera.transform.LookAt(_bossTransform);

                distance = Vector3.Distance(_directingCamera.transform.position, _endPosition);
                await UniTask.Yield(token);
            }

            ////補間
            _directingCamera.transform.position = _endPosition;
            _directingCamera.transform.LookAt(_bossTransform);
            await UniTask.Yield(token);
        }
    }
}
