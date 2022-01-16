using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UniRx;
using TimeModel;
using GameModel;
using TimeView;

namespace TimePresenter
{
    public class TimePresenter : MonoBehaviour
    {
        [SerializeField]
        [Header("制限カウントを設定")]
        int _gameLimitCountTime = 60;

        #region//フィールド
        CountDownTimer _countDownTimer;//カウントダウン処理
        TimeView.TimeView _timeView;//タイマー表示用のUI
        ITimeModel _timeModel;
        IGameModel _gameModel;
        #endregion

        /// <summary>
        /// プレハブのインスタンス直後の処理
        /// </summary>
        public void ManualAwake()
        {
            _countDownTimer = GetComponent<CountDownTimer>();
            _timeView = GetComponent<TimeView.TimeView>();
        }

        [Inject]
        public void Construct(
            ITimeModel timeModel,
            IGameModel gameModel
        )
        {
            _timeModel = timeModel;
            _gameModel = gameModel;
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        public void Initialize()
        {
            _timeModel.SetTime(_gameLimitCountTime);
            _countDownTimer.SetCountTime(_gameLimitCountTime);
            _countDownTimer.Publish();//ゲーム開始までカウントを待機
            Bind();
        }

        /// <summary>
        /// modelとviewの監視、処理
        /// </summary>
        void Bind()
        {
            //ゲーム開始でカウント開始
            _gameModel.IsGameStart
                .Where(can => can == true)
                .Subscribe(_ => _countDownTimer.Connect());

            //カウントダウン
            _countDownTimer.CountDownObservable
                .Where(_ => _gameModel.CanGame())
                .Subscribe(time =>
                {
                    _timeModel.SetTime(time);
                },
                () => {
                    //カウント終了でゲームオーバー
                    _timeModel.SetTime(0);
                    _gameModel.SetIsGameOver(true);
                });

            //Model to View
            _timeModel.Time.Subscribe(time =>
            {
                _timeView.SetTimeText(time);
            });
        }
    }
}