using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UniRx;
using TimeModel;
using TimeView;

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

        [SerializeField]
        [Header("タイマー表示用のUIを設定")]
        TimeView.TimeView _timeView;

        #region//フィールド
        ITimeModel _timeModel;
        BoolReactiveProperty _canStartGame = new BoolReactiveProperty();//ゲーム開始フラグ
        BoolReactiveProperty _isGameOver = new BoolReactiveProperty();//ゲームオーバー
        #endregion

        #region//プロパティ
        public IReadOnlyReactiveProperty<bool> IsGameOver => _isGameOver;
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
            _countDownTimer.Publish();//ゲーム開始までカウントを待機
            Bind();
        }

        /// <summary>
        /// modelとviewの監視、処理
        /// </summary>
        void Bind()
        {
            //ゲーム開始でカウント開始
            _canStartGame
                .Where(can => can == true)
                .Subscribe(_ => _countDownTimer.Connect());

            //カウントダウン
            _countDownTimer.CountDownObservable
                .Where(_ => CanGame())
                .Subscribe(time =>
                {
                    _timeModel.SetTime(time);
                },
                () => {
                    //カウント終了でゲームオーバー
                    _timeModel.SetTime(0);
                    _isGameOver.Value = true;
                });

            //Model to View
            _timeModel.Time.Subscribe(time =>
            {
                _timeView.SetTimeText(time);
            });
        }

        // <summary>
        /// ゲーム開始フラグの設定
        /// </summary>
        /// <param name="can"></param>
        public void SetCanStartGame(bool can)
        {
            _canStartGame.Value = can;
        }

        /// <summary>
        /// ゲームオーバーフラグの設定
        /// </summary>
        public void SetIsGameOver(bool isGameOver)
        {
            _isGameOver.Value = isGameOver;
        }

        /// <summary>
        /// ゲームができるか
        /// </summary>
        /// <returns></returns>
        bool CanGame()
        {
            return (_canStartGame.Value && _isGameOver.Value == false);
        }
    }
}