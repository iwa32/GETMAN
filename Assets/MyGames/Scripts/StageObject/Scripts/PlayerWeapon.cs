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
        }

        public void StartMotion()
        {
            _soundManager.PlaySE(SWORD_SLASH);
            EnableCollider();
        }

        public void EndMotion()
        {
            UnEnableCollider();
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