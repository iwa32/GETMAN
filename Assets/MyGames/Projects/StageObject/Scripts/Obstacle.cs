using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using GameModel;
using GlobalInterface;
using HpBar;
using Trigger;
using Zenject;

namespace StageObject
{
    /// <summary>
    /// 障害物
    /// </summary>
    public abstract class Obstacle : MonoBehaviour, IScore
    {
        //HPバーの表示時間
        const float HPBAR_DISPLAY_TIME = 3.0f;

        [SerializeField]
        [Header("耐久値を設定")]
        int _enduranceValue;

        [SerializeField]
        [Header("HpBarを設定")]
        HpBar.HpBar _hpBar;

        [SerializeField]
        [Header("獲得スコアを設定")]
        int _score;

        ObservableTrigger _observableTrigger;
        bool _isHit;
        IScoreModel _scoreModel;//gameの保持するスコア

        public int Score => _score;

        [Inject]
        public void Construct(IScoreModel scoreModel)
        {
            _scoreModel = scoreModel;
        }

        void Awake()
        {
            _observableTrigger = GetComponent<ObservableTrigger>();
        }

        void Start()
        {
            _hpBar.SetMaxHp(_enduranceValue);
            _hpBar.SetHp(_enduranceValue);
            _hpBar.gameObject?.SetActive(false);
            Bind();
        }

        void Bind()
        {
            //接触
            _observableTrigger.OnTriggerEnter()
                .Subscribe(trigger => CheckTrigger(trigger))
                .AddTo(this);
        }

        void CheckTrigger(Collider collider)
        {
            CheckDamegedBy(collider);
        }

        void CheckDamegedBy(Collider collider)
        {
            if (_isHit) return;//1フレーム内での連続ヒット防止
            //プレイヤーの攻撃を受けます
            if (collider.TryGetComponent(out IEnemyAttacker attaker))
                Damaged(attaker.Power);
        }

        /// <summary>
        /// ダメージを受けます
        /// </summary>
        /// <param name="damage"></param>
        void Damaged(int damage)
        {
            _isHit = true;
            RefreshHit().Forget();

            _enduranceValue -= damage;
            _hpBar.SetHp(_enduranceValue);

            if (_hpBar.gameObject.activeInHierarchy == false)
                HideHpBarAfterDisplayTime().Forget();
            if (_enduranceValue <= 0)
            {
                Destroy();
                _scoreModel.AddScore(_score);
            }
                
        }

        /// <summary>
        /// 指定時間HPバーを表示します
        /// </summary>
        /// <returns></returns>
        async UniTask HideHpBarAfterDisplayTime()
        {
            _hpBar.gameObject?.SetActive(true);
            await UniTask.Delay(TimeSpan.FromSeconds(HPBAR_DISPLAY_TIME));
            _hpBar.gameObject?.SetActive(false);
        }

        async UniTask RefreshHit()
        {
            await UniTask.Yield();
            _isHit = false;
        }

        #region//abstract method
        protected abstract void Destroy();
        #endregion
    }
}
