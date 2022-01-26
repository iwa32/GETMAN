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

namespace Title
{
    public class Title : MonoBehaviour
    {
        [SerializeField]
        [Header("スタートボタンのテキスト")]
        TextMeshProUGUI _startButtonText;

        [SerializeField]
        [Header("StartButtonを設定")]
        Button _startButton;

        [SerializeField]
        [Header("ボタンの点滅速度")]
        float _blinkSpeed = 1.0f;

        [SerializeField]
        [Header("ボタンの点滅回数")]
        float _blinkCount = 5.0f;

        [SerializeField]
        [Header("ボタンの点滅時間")]
        float _maxBlinkTime = 1.0f;

        [SerializeField]
        [Header("クリック時のSE")]
        AudioClip _clickSE;


        float _elapsedBlinkTime;
        float _blinkTime;


        #region//フィールド
        ICustomSceneManager _customSceneManager;
        IObservableClickButton _observableClickButton;
        #endregion

        [Inject]
        public void Construct(
            ICustomSceneManager customSceneManager,
            IObservableClickButton observableClickButton
        )
        {
            _customSceneManager = customSceneManager;
            _observableClickButton = observableClickButton;
        }

        void Start()
        {
            Bind();
        }

        void Bind()
        {
            _observableClickButton
                .CreateObservableClickButton(_startButton)
                .Subscribe(_ =>
                {
                    StartGame().Forget();
                })
                .AddTo(this);
        }

        async UniTask StartGame()
        {
            await BlinkClickedButton();

            _customSceneManager.LoadScene(SceneType.STAGE);
        }

        /// <summary>
        /// ボタンの点滅
        /// </summary>
        /// <returns></returns>
        async UniTask BlinkClickedButton()
        {
            while (_blinkTime <= _maxBlinkTime)
            {
                _startButtonText.color = GetAlphaColor(_startButtonText.color);
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