using System;
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
        BoolReactiveProperty _canStartGame = new BoolReactiveProperty();//ゲーム開始フラグ
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
            CanGame()
                .Where(can => can == true)
                .Subscribe(_ => _countDownTimer.Connect());

            //カウントダウン
            _countDownTimer.CountDownObservable
                .Subscribe(time =>
                {
                    Debug.Log(time);
                });

            //カウント終了でゲームオーバー
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
        /// ゲームができるか
        /// </summary>
        /// <returns></returns>
        IObservable<bool> CanGame()
        {
            //return (_canStartGame && _isGameOver.Value == false);
            return _canStartGame;
        }
    }
}