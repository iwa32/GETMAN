using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using NormalPlayerWeapon;
using SpWeaponDataList;
using PlayerView;
using SPWI = SpPlayerWeaponInvoker;
using Zenject;
using UnityEngine.AI;
using SpPlayerWeaponInvokerPool;

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
        NavMeshAgent _navMeshAgent;
        bool _isBlink;//点滅状態か
        SPWI.SpPlayerWeaponInvoker _currentSpWeapon;//現在取得しているSP武器を保持
        ISpPlayerWeaponInvokerPool _invokerPool;//取得したSP武器呼び出し用クラスを保持

        public SpWeaponData SpWeaponData => _spWeaponData;
        public bool IsBlink => _isBlink;

        
        [Inject]
        public void Construct(ISpPlayerWeaponInvokerPool invokerPool)
        {
            _invokerPool = invokerPool;
        }

        /// <summary>
        /// プレハブのインスタンス直後の処理
        /// </summary>
        public void ManualAwake()
        {
            _rigidBody = GetComponent<Rigidbody>();
            _inputView = GetComponent<InputView>();
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _mainCamera = Camera.main;
        }

        public void Initialize()
        {
            //navMeshの警告エラーの対処
            _navMeshAgent.enabled = true;
        }

        public void SetTransform(Transform startingTransform)
        {
            _navMeshAgent.Warp(startingTransform.position);
            transform.rotation = startingTransform.rotation;
        }

        /// <summary>
        /// 通常攻撃を行います
        /// </summary>
        public void StartNormalAttack()
        {
            _playerWeapon.StartMotion();
        }

        /// <summary>
        /// 通常攻撃をやめます
        /// </summary>
        public void EndNormalAttack()
        {
            _playerWeapon.EndMotion();
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
            if (_currentSpWeapon?.Type == _spWeaponData.Type) return;

            _currentSpWeapon?.gameObject?.SetActive(false);//現在のSP武器を非表示にする
            _currentSpWeapon = _invokerPool.GetPool(type);

            //poolにない場合、新規作成する
            if (_currentSpWeapon == null)
            {
                _invokerPool.CreatePool(_spWeaponData.SpWeaponInvoker);
                _currentSpWeapon = _invokerPool.GetPool(_spWeaponData.Type);
                _currentSpWeapon.SetPower(_spWeaponData.Power);
                _currentSpWeapon.SetPlayerTransform(transform);
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
            if (this == null) return;//シーン遷移による破棄後は何もしないようにする
            foreach (Transform child in gameObject.transform)
            {
                child?.gameObject?.SetActive(isActive);
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
            transform.LookAt(_mainCamera.transform.position);
        }
    }
}