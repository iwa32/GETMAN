using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using SoundManager;
using static SEType;

namespace StageObject
{
    public class PlayerWeapon : MonoBehaviour,
    IPlayerWeapon
    {
        [SerializeField]
        [Header("武器の攻撃力を設定")]
        int _power;

        [SerializeField]
        [Header("エフェクトのトレイルを設定")]
        TrailRenderer _trailRenderer;

        Collider _collider;
        ISoundManager _soundManager;

        public int Power => _power;

        void Awake()
        {
            _collider = GetComponent<Collider>();
        }

        [Inject]
        void Construct(ISoundManager soundManager)
        {
            _soundManager = soundManager;
        }

        public void Initialize()
        {
            //武器判定をオフに
            UnEnableCollider();
            _trailRenderer.emitting = false;
        }

        public void StartMotion()
        {
            _soundManager.PlaySE(SWORD_SLASH);
            EnableCollider();
            _trailRenderer.emitting = true;
        }

        public void EndMotion()
        {
            UnEnableCollider();
            _trailRenderer.emitting = false;
        }

        /// <summary>
        /// コライダーを有効にする
        /// </summary>
        void EnableCollider()
        {
            _collider.enabled = true;
        }

        /// <summary>
        /// コライダーを無効にする
        /// </summary>
        void UnEnableCollider()
        {
            _collider.enabled = false;
        }

        /// <summary>
        /// パワーの設定
        /// </summary>
        /// <param name="power"></param>
        public void SetPower(int power)
        {
            _power = power;
        }
    }
}