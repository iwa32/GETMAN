using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Cysharp.Threading.Tasks;
using Zenject;
using CustomSceneManager;
using UIUtility;
using TMPro;
using Fade;
using SoundManager;
using SaveDataManager;

namespace Title
{
    public class Title : MonoBehaviour
    {
        [SerializeField]
        [Header("スタートゲームボタンのテキスト")]
        TextMeshProUGUI _startGameButtonText;

        [SerializeField]
        [Header("ハイスコア表示用テキスト")]
        TextMeshProUGUI _highScoreText;

        [SerializeField]
        [Header("StartGameButtonを設定")]
        Button _startGameButton;

        [SerializeField]
        [Header("RankingGameButtonを設定")]
        Button _rankingButton;

        [SerializeField]
        [Header("Ranking表示Canvasを設定")]
        RankingPresenter.RankingPresenter _rankingCanvas;

        [SerializeField]
        [Header("ボタンの点滅速度")]
        float _blinkSpeed = 1.0f;

        [SerializeField]
        [Header("ボタンの点滅回数")]
        float _blinkCount = 5.0f;

        [SerializeField]
        [Header("ボタンの点滅時間")]
        float _maxBlinkTime = 1.0f;


        float _elapsedBlinkTime;
        float _blinkTime;
        bool _isClickedButton;


        #region//フィールド
        ICustomSceneManager _customSceneManager;
        ISaveDataManager _saveDataManager;
        ISoundManager _soundManager;
        IObservableClickButton _observableClickButton;
        IFade _fade;
        #endregion

        [Inject]
        public void Construct(
            ICustomSceneManager customSceneManager,
            ISaveDataManager saveDataManager,
            ISoundManager soundManager,
            IObservableClickButton observableClickButton,
            IFade fade
        )
        {
            _customSceneManager = customSceneManager;
            _saveDataManager = saveDataManager;
            _soundManager = soundManager;
            _observableClickButton = observableClickButton;
            _fade = fade;
        }

        void Start()
        {
            _fade.StartFadeIn().Forget();
            ShowHighScore();
            Bind();
        }

        void Bind()
        {
            //ゲーム開始ボタン
            _observableClickButton
                .CreateObservableClickButton(_startGameButton)
                .First()
                .Subscribe(_ =>
                {
                    StartGame().Forget();
                })
                .AddTo(this);

            //ランキング表示ボタン
            _observableClickButton
                .CreateObservableClickButton(_rankingButton)
                .Subscribe(_ =>
                {
                    _rankingCanvas.ShowRanking();
                })
                .AddTo(this);
        }

        /// <summary>
        /// ハイスコアの表示
        /// </summary>
        void ShowHighScore()
        {
            if (_saveDataManager.IsLoaded == false) return;

            _highScoreText.text = "HighScore: " + _saveDataManager.SaveData.HighScore.ToString();
        }

        /// <summary>
        /// ゲームの開始
        /// </summary>
        /// <returns></returns>
        async UniTask StartGame()
        {
            if (_isClickedButton) return;
            _isClickedButton = true;

            _soundManager.PlaySE(SEType.SCENE_MOVEMENT);
            await BlinkClickedButton(_startGameButtonText);

            _saveDataManager.SetIsInitialized(true);

            await UniTask.Yield();

            _customSceneManager.LoadScene(SceneType.STAGE);
        }

        /// <summary>
        /// ボタンの点滅
        /// </summary>
        /// <returns></returns>
        async UniTask BlinkClickedButton(TextMeshProUGUI text)
        {
            while (_blinkTime <= _maxBlinkTime)
            {
                text.color = GetAlphaColor(text.color);
                _blinkTime += Time.deltaTime;
                await UniTask.Delay(TimeSpan.FromSeconds(Time.deltaTime));
            }
        }

        /// <summary>
        /// アルファ値を取得
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        Color GetAlphaColor(Color color)
        {
            _elapsedBlinkTime += Time.deltaTime * _blinkCount * _blinkSpeed;
            color.a = Mathf.Sin(_elapsedBlinkTime);
            return color;
        }
    }

}