using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Zenject;
using SoundManager;
using static SEType;
using Trigger;
using UniRx;

namespace NormalPlayerWeapon
{
    public class PlayerSword : MonoBehaviour,
    IPlayerWeapon
    {
        [SerializeField]
        [Header("武器の攻撃力を設定")]
        int _power;

        [SerializeField]
        [Header("トレイルをまとめたゲームオブジェクトを設定")]
        GameObject _trailGroups;

        Collider _collider;
        //---接触・衝突---
        ObservableTrigger _trigger;
        ISoundManager _soundManager;

        public int Power => _power;

        void Awake()
        {
            _collider = GetComponent<Collider>();
            //接触、衝突
            _trigger = GetComponent<ObservableTrigger>();
        }

        [Inject]
        void Construct(ISoundManager soundManager)
        {
            _soundManager = soundManager;
        }

        void Start()
        {
            Initialize();
            Bind();
        }

        void Initialize()
        {
            //武器判定をオフに
            _collider.enabled = false;
            _trailGroups.SetActive(false);
        }

        void Bind()
        {
            //trigger, collisionの取得
            _trigger.OnTriggerEnter()
                .Where(colider => colider.CompareTag("Enemy"))
                .Subscribe(collider => {
                    Hit();
                })
                .AddTo(this);
        }

        void Hit()
        {
            _soundManager.PlaySE(SLASHED);
        }

        public void StartMotion()
        {
            _soundManager.PlaySE(SWORD_SLASH);
            _collider.enabled = true;
            _trailGroups.SetActive(true);
        }

        public void EndMotion()
        {
            _collider.enabled = false;
            _trailGroups.SetActive(false);
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