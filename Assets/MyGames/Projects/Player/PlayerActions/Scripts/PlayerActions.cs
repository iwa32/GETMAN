using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using NormalPlayerWeapon;
using SpWeaponDataList;
using PlayerView;
using SpPlayerWeaponInvoker;
using Zenject;

namespace PlayerActions {
    /// <summary>
    /// プレイヤーの実行処理
    /// </summary>
    public class PlayerActions : MonoBehaviour
    {
        [SerializeField]
        [Header("プレイヤーの点滅時間")]
        float _blinkTime = 3.0f;

        [SerializeField]
        [Header("ノックバック時の飛ぶ威力")]
        float _knockBackPower = 10.0f;

        [SerializeField]
        [Header("プレイヤーの移動速度")]
        float _speed = 10.0f;

        [SerializeField]
        [Header("装備武器を設定")]
        PlayerSword _playerWeapon;

        [SerializeField]
        [Header("SpWeaponのScritableObjectを設定")]
        SpWeaponDataList.SpWeaponDataList _spWeaponDataList;

        Camera _mainCamera;
        InputView _inputView;//プレイヤーの入力取得スクリプト
        Rigidbody _rigidBody;
        SpWeaponData _spWeaponData;
        bool _isBlink;//点滅状態か
        ISpPlayerWeaponInvoker _currentSpWeapon;//現在取得しているSP武器を保持

        public SpWeaponData SpWeaponData => _spWeaponData;
        public bool IsBlink => _isBlink;

        [Inject]
        DiContainer container;//動的生成したデータにDIできるようにする

        /// <summary>
        /// プレハブのインスタンス直後の処理
        /// </summary>
        public void ManualAwake()
        {
            _rigidBody = GetComponent<Rigidbody>();
            _inputView = GetComponent<InputView>();
            _mainCamera = Camera.main;
        }

        /// <summary>
        /// 通常攻撃を行います
        /// </summary>
        public void DoNormalAttack()
        {
            _playerWeapon.Use();
        }

        /// <summary>
        /// SP攻撃を行います
        /// </summary>
        public void DoSpAttack()
        {
            if (_currentSpWeapon == null) return;

            _currentSpWeapon.Invoke();
        }

        /// <summary>
        /// SP武器を設定します
        /// </summary>
        /// <param name="type"></param>
        public void SetSpWeapon(SpWeaponType type)
        {
            _spWeaponData = _spWeaponDataList.FindSpWeaponDataByType(type);

            //武器が違う場合のみセットする
            if (_currentSpWeapon?.Type != _spWeaponData.Type)
            {
                ISpPlayerWeaponInvoker invoker =
                    container.InstantiatePrefab(_spWeaponData.SpWeaponInvoker)
                    .GetComponent<ISpPlayerWeaponInvoker>();

                _currentSpWeapon = invoker;
                _currentSpWeapon.SetPlayerTransform(transform);
                _currentSpWeapon.SetPower(_spWeaponData.Power);
            }
        }

        /// <summary>
        /// ノックバックします
        /// </summary>
        public void KnockBack(GameObject target)
        {
            //ノックバック方向を取得
            Vector3 knockBackDirection = (transform.position - target.transform.position).normalized;

            //速度ベクトルをリセット
            _rigidBody.velocity = Vector3.zero;
            knockBackDirection.y = 0;//Y方向には飛ばないようにする
            _rigidBody.AddForce(knockBackDirection * _knockBackPower, ForceMode.VelocityChange);
        }

        /// <summary>
        /// プレイヤーの点滅
        /// </summary>
        public async UniTask PlayerBlinks()
        {
            bool isActive = false;
            float elapsedBlinkTime = 0.0f;

            _isBlink = true;
            while (elapsedBlinkTime <= _blinkTime)
            {
                SetActiveToAllChild(isActive);
                isActive = !isActive;
                await UniTask.Delay(TimeSpan.FromSeconds(0.2f));
                elapsedBlinkTime += 0.2f;
            }

            SetActiveToAllChild(true);
            _isBlink = false;
        }

        /// <summary>
        /// 子要素を全てアクティブ・非アクティブにする
        /// </summary>
        /// <param name="isActive"></param>
        void SetActiveToAllChild(bool isActive)
        {
            foreach (Transform child in gameObject.transform)
            {
                child.gameObject.SetActive(isActive);
            }
        }

        /// <summary>
        /// 走ります
        /// </summary>
        public void Run()
        {
            Vector2 input = _inputView.InputDirection.Value;
            Move(input);
            Rotation(input);
        }

        /// <summary>
        /// 移動します
        /// </summary>
        /// <param name="input"></param>
        void Move(Vector2 input)
        {
            //入力があった場合
            if (input != Vector2.zero)
            {
                Vector3 movePos = new Vector3(input.x, 0, input.y);
                _rigidBody.velocity = movePos * _speed;
            }
        }

        /// <summary>
        /// 回転します
        /// </summary>
        /// <param name="input"></param>
        void Rotation(Vector2 input)
        {
            _rigidBody.rotation = Quaternion.LookRotation(new Vector3(input.x, 0, input.y));
        }

        /// <summary>
        /// カメラの方を向きます
        /// </summary>
        public void LookAtCamera()
        {
            transform.LookAt(-_mainCamera.transform.forward);
        }
    }
}