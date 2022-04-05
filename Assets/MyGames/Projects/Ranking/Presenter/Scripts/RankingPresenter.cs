using System.Collections;
using System.Collections.Generic;
using AuthManager;
using UnityEngine;
using RankingModel;
using Zenject;
using SoundManager;
using UIUtility;
using UnityEngine.UI;
using UniRx;
using RankingView;
using Cysharp.Threading.Tasks;
using System;
using TMPro;

namespace RankingPresenter
{
    public class RankingPresenter : MonoBehaviour
    {
        [SerializeField]
        [Header("ランキングを閉じるボタンを設定")]
        Button _closeRankingButton;

        [SerializeField]
        [Header("ランキングの登録ボタンを設定")]
        Button _registerRankingButton;

        [SerializeField]
        [Header("ユーザー名入力フィールドを設定")]
        TMP_InputField _userNameInputField;

        [SerializeField]
        [Header("ランキングデータのプレハブを設定")]
        RankingUserDataView _rankingUserDataPrefab;

        [SerializeField]
        [Header("ランキングデータの表示領域を設定")]
        GameObject _content;

        [SerializeField]
        [Header("ランキングの表示数を設定")]
        int _maxResultCount = 3;

        [SerializeField]
        [Header("ユーザー名の最大文字数を設定")]
        int _maxLengthOfUserName = 8;

        [SerializeField]
        [Header("ユーザ名に不備があるときのアラートメッセージを設定")]
        TextMeshProUGUI _alertMessage;

        #region//フィールド
        IAuthManager _authManager;
        IRankingModel _rankingModel;
        IToggleableUI _toggleableUI;
        ISoundManager _soundManager;
        IObservableClickButton _observableClickButton;
        IObservableInputField _observableInputField;
        RankingUserDataView[] _rankingUserDataPool;//ランキングviewを保管しておく
        bool _isValidUserName;
        string _checkedUserName;
        //CancellationTokenSource _cts = new CancellationTokenSource();
        #endregion

        [Inject]
        public void Construct(
            IAuthManager authManager,
            IRankingModel rankingModel,
            IToggleableUI toggleableUI,
            ISoundManager soundManager,
            IObservableClickButton observableClickButton,
            IObservableInputField observableInputField
        )
        {
            _authManager = authManager;
            _rankingModel = rankingModel;
            _toggleableUI = toggleableUI;
            _soundManager = soundManager;
            _observableClickButton = observableClickButton;
            _observableInputField = observableInputField;
        }

        // Start is called before the first frame update
        void Start()
        {
            //CloseRanking();
            Initialize();
            SetRankingDataToView().Forget();
            Bind();
        }

        private void Initialize()
        {
            //取得数の設定 取得数は定数にするか迷い中
            _rankingModel.SetMaxResultCount(_maxResultCount);
            PreCreateRankingDataView();
        }

        void Bind()
        {
            //ランキング非表示ボタン
            _observableClickButton.
                CreateObservableClickButton(_closeRankingButton)
                .Subscribe(_ => CloseRanking())
                .AddTo(this);

            //ユーザ名入力フィールド
            _observableInputField.
                CreateObservableInputFieldOnEndEdit(_userNameInputField)
                .Subscribe(value => CheckUserName(value))
                .AddTo(this);

            //ランキング登録ボタン
            _observableClickButton.
                CreateObservableClickButton(_registerRankingButton)
                .Subscribe(_ => RegisterUserData())
                .AddTo(this);
        }

        /// <summary>
        /// ランキングコンポーネントを事前に作成する
        /// </summary>
        public void PreCreateRankingDataView()
        {
            _rankingUserDataPool = new RankingUserDataView[_maxResultCount];

            //表示数分作成しておく
            for (int i = 0; i < _maxResultCount; i++)
            {
                RankingUserDataView userView = Instantiate(_rankingUserDataPrefab, _content.transform);
                _rankingUserDataPool[i] = userView;
            }
            //最初は隠しておく
            _content.SetActive(false);


            //todo ＋自分の作成
            //非表示にしておく
        }

        /// <summary>
        /// viewにランキングを表示します
        /// </summary>
        /// <returns></returns>
        async UniTask SetRankingDataToView()
        {
            try
            {
                //todo ローディングなど
                //ログインを待ちます
                await UniTask.WaitUntil(() => _authManager.IsLoggedIn || _authManager.IsError);
                if (_authManager.IsError)
                    throw new OperationCanceledException();

                await _rankingModel.LoadRankingList();

                //取得数を基準に周します
                //例:3件まで表示できるが実際の取得数が2件の場合もあるため
                for (int i = 0; i < _rankingModel.RankingList.Count; i++)
                {
                    _rankingUserDataPool[i].SetRank(_rankingModel.RankingList[i]._rank);
                    _rankingUserDataPool[i].SetUserName(_rankingModel.RankingList[i]._userName);
                    _rankingUserDataPool[i].SetScore(_rankingModel.RankingList[i]._score);
                }

                _content.SetActive(true);
            }
            catch (OperationCanceledException)
            {
                //todo エラーダイアログを表示する
                Debug.Log("errorだお");
            }
            
        }

        /// <summary>
        /// ユーザー名が有効かチェックする
        /// </summary>
        /// <param name="value"></param>
        void CheckUserName(string value)
        {
            //バリデーション
            (bool, string) checkedValidityAndMessage
                = _observableInputField.CheckMaxLength(value, _maxLengthOfUserName);

            _isValidUserName = checkedValidityAndMessage.Item1;
            _checkedUserName = value;

            //表示するメッセージが違う場合のみ描画する
            if (_alertMessage.text != checkedValidityAndMessage.Item2)
            {
                _alertMessage.text = checkedValidityAndMessage.Item2;
            }
        }

        /// <summary>
        /// ユーザーデータの登録
        /// </summary>
        void RegisterUserData()
        {
            if (_isValidUserName == false) return;
            _rankingModel.RegisterUserName(_checkedUserName);

            //完了したらダイアログを出す
            //スコアの登録
            //ランキングの再読み込み
        }

        /// <summary>
        /// ランキングの表示
        /// </summary>
        public void ShowRanking()
        {
            _soundManager.PlaySE(SEType.COMMON_BUTTON_CLICK);
            _toggleableUI.OpenUIFor(gameObject);
        }

        /// <summary>
        /// ランキングの非表示
        /// </summary>
        public void CloseRanking()
        {
            _toggleableUI.CloseUIFor(gameObject);
        }
    }
}