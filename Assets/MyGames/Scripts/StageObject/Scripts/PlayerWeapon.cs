using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StageObject
{
    public class PlayerWeapon : MonoBehaviour,
    IPlayerWeapon
    {
        [SerializeField]
        [Header("武器の攻撃力を設定")]
        int _power;

        Collider _collider;

        public int Power => _power;

        void Awake()
        {
            _collider = GetComponent<Collider>();
        }

        /// <summary>
        /// コライダーを有効にする
        /// </summary>
        public void EnableCollider()
        {
            _collider.enabled = true;
        }

        /// <summary>
        /// コライダーを無効にする
        /// </summary>
        public void UnEnableCollider()
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