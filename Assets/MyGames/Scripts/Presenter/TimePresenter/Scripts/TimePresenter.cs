using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UniRx;
using UniRx.Triggers;
using TimeModel;
using GameModel;
using CountDownTimer;

namespace TimePresenter
{
    public class TimePresenter : MonoBehaviour
    {
        [SerializeField]
        [Header("制限カウントを設定")]
        int _gameLimitCountTime = 60;

        #region//フィールド
        TimeView.TimeView _timeView;//タイマー表示用のUI
        ITimeModel _timeModel;
        IDirectionModel _directionModel;
        IObservableCountDownTimer _countDownTimer;//カウントダウン処理
        #endregion

        /// <summary>
        /// プレハブのインスタンス直後の処理
        /// </summary>
        public void ManualAwake()
        {
            _timeView = GetComponent<TimeView.TimeView>();
        }

        [Inject]
        public void Construct(
            ITimeModel timeModel,
            IDirectionModel directionModel,
            IObservableCountDownTimer countDownTimer
        )
        {
            _timeModel = timeModel;
            _directionModel = directionModel;
            _countDownTimer = countDownTimer;
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
            _directionModel.IsGameStart
                .Where(can => can == true)
                .Subscribe(_ => _countDownTimer.Connect())
                .AddTo(this);

            //カウントダウン
            IDisposable countDownDisposable
                = _countDownTimer.CountDownObservable
                .Where(_ => _directionModel.CanGame())
                .Subscribe(time =>
                {
                    _timeModel.SetTime(time);
                },
                () => {
                    //カウント終了でゲームオーバー
                    _timeModel.SetTime(0);
                    _directionModel.SetIsGameOver(true);
                });

            //ゲーム終了でカウント終了
            this.UpdateAsObservable()
                .First(_ => _directionModel.IsEndedGame())
                .Subscribe(_ =>
                {
                    countDownDisposable.Dispose();
                })
                .AddTo(this);

            //Model to View
            _timeModel.Time.Subscribe(time =>
            {
                _timeView.SetTimeText(time);
            });
        }
    }
}