using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Zenject;
using TMPro;
using UIUtility;
using UnityEngine.UI;
using SoundManager;
using static SEType;

namespace Dialog
{
    public class Dialog : MonoBehaviour, IDialog
    {
        [SerializeField]
        [Header("メッセージのテキストを設定")]
        TextMeshProUGUI _messageText;

        [SerializeField]
        [Header("オーバーレイのオブジェクトを設定")]
        Button _overlay;

        [SerializeField]
        [Header("閉じるボタンを設定")]
        Button _closeButton;

        IObservableClickButton _observableClickButton;
        ISoundManager _soundManager;
        IToggleableUI _toggleableUI;

        void Start()
        {
            Bind();
            Initialize();
        }

        void Initialize()
        {
            _toggleableUI.CloseUIFor(gameObject);
        }

        [Inject]
        public void Construct(
            IObservableClickButton observableClickButton,
            ISoundManager soundManager,
            IToggleableUI toggleableUI
        )
        {
            _observableClickButton = observableClickButton;
            _soundManager = soundManager;
            _toggleableUI = toggleableUI;
        }

        void Bind()
        {
            //ダイアログを閉じる
            _observableClickButton
                .CreateObservableClickButton(_closeButton)
                .Subscribe(_ => CloseDialog())
                .AddTo(this);

            _observableClickButton
                .CreateObservableClickButton(_overlay)
                .Subscribe(_ => CloseDialog())
                .AddTo(this);
        }

        public void SetText(string text)
        {
            _messageText.text = text;
        }

        public void OpenDialog()
        {
            _toggleableUI.OpenUIFor(gameObject);
        }

        public void CloseDialog()
        {
            _soundManager.PlaySE(COMMON_BUTTON_CLICK);
            _toggleableUI.CloseUIFor(gameObject);
            SetText("");//文字を空にする
        }
    }
}