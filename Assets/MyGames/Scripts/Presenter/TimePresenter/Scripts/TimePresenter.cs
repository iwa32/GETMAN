using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UniRx;
using TimeModel;

namespace TimePresenter
{
    public class TimePresenter : MonoBehaviour
    {
        [SerializeField]
        [Header("制限カウントを設定")]
        int _gameLimitCountTime = 60;

        [SerializeField]
        [Header("カウントダウン処理を設定")]
        CountDownTimer _countDownTimer;

        #region//フィールド
        ITimeModel _timeModel;
        #endregion

        [Inject]
        public void Construct(ITimeModel timeModel)
        {
            _timeModel = timeModel;
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        public void Initialize()
        {
            _timeModel.SetTime(_gameLimitCountTime);
            _countDownTimer.SetCountTime(_gameLimitCountTime);
            Bind();
        }

        /// <summary>
        /// modelとviewの監視、処理
        /// </summary>
        void Bind()
        {
            //ゲーム開始で、カウント
            //カウント終了でゲームオーバー
        }
    }
}