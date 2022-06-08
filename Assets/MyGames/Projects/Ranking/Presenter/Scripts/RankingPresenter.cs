using AuthManager;
using Cysharp.Threading.Tasks;
using Dialog;
using Loading;
using RankingModel;
using UniRx;
using UIUtility;
using UnityEngine;
using UnityEngine.UI;
using RankingView;
using SaveDataManager;
using SoundManager;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Zenject;

namespace RankingPresenter
{
    public class RankingPresenter : MonoBehaviour
    {
        [SerializeField]
        [Header("ランキングを閉じるボタンを設定")]
        Button _buttonToCloseRanking;

        [SerializeField]
        [Header("ランキングの登録ボタンを設定")]
        Button _buttonToRegisterRanking;

        [SerializeField]
        [Header("ランキングの再読み込みボタンを設定")]
        Button _reloadButton;

        [SerializeField]
        [Header("ユーザー名入力フィールドを設定")]
        TMP_InputField _userNameInputField;

        [SerializeField]
        [Header("自身のランキングデータ表示用オブジェクトを設定")]
        RankingUserDataView _myRankingDataObject;

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

        [SerializeField]
        [Header("登録用ハイスコアを表示するテキストの設定")]
        TextMeshProUGUI _registeringHighScoreValueText;

        #region//フィールド
        IAuthManager _authManager;
        ISuccessDialog _successDialog;
        IErrorDialog _errorDialog;
        IRankingModel _rankingModel;
        IToggleableUI _toggleableUI;
        ISoundManager _soundManager;
        IObservableClickButton _observableClickButton;
        IObservableInputField _observableInputField;
        ISaveDataManager _saveDataManager;
        ILoading _loading;
        RankingUserDataView[] _rankingUserDataPool;//ランキングviewを保管しておく
        bool _isValidUserName;
        string _checkedUserName;
        //CancellationTokenSource _cts = new CancellationTokenSource();
        #endregion

        [Inject]
        public void Construct(
            IAuthManager authManager,
            ISuccessDialog successDialog,
            IErrorDialog errorDialog,
            IRankingModel rankingModel,
            IToggleableUI toggleableUI,
            ISoundManager soundManager,
            IObservableClickButton observableClickButton,
            IObservableInputField observableInputField,
            ISaveDataManager saveDataManager,
            ILoading loading
        )
        {
            _authManager = authManager;
            _successDialog = successDialog;
            _errorDialog = errorDialog;
            _rankingModel = rankingModel;
            _toggleableUI = toggleableUI;
            _soundManager = soundManager;
            _observableClickButton = observableClickButton;
            _observableInputField = observableInputField;
            _saveDataManager = saveDataManager;
            _loading = loading;
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
            //取得数の設定
            _rankingModel.SetMaxResultCount(_maxResultCount);
            _registeringHighScoreValueText.text = _saveDataManager.SaveData.HighScore.ToString();
            PreCreateRankingDataView();
        }

        void Bind()
        {
            //ランキング非表示ボタン
            _observableClickButton.
                CreateObservableClickButton(_buttonToCloseRanking)
                .Subscribe(_ => CloseRanking())
                .AddTo(this);

            //ユーザ名入力フィールド
            _observableInputField.
                CreateObservableInputFieldOnEndEdit(_userNameInputField)
                .Subscribe(value => CheckUserName(value))
                .AddTo(this);

            //ランキング登録ボタン
            _observableClickButton.
                CreateObservableClickButton(_buttonToRegisterRanking)
                .Subscribe(_ => RegisterUserData().Forget())
                .AddTo(this);

            //ランキング再読み込みボタン
            _observableClickButton.
                CreateObservableClickButton(_reloadButton)
                .Subscribe(_ => ReloadRankingData())
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

        }

        /// <summary>
        /// ランキングデータの再読み込み
        /// </summary>
        void ReloadRankingData()
        {
            _soundManager.PlaySE(SEType.COMMON_BUTTON_CLICK);
            SetRankingDataToView().Forget();
        }

        /// <summary>
        /// viewにランキングを表示します
        /// </summary>
        /// <returns></returns>
        async UniTask SetRankingDataToView()
        {
            try
            {
                _loading.OpenLoading();
                //ログインを待ちます
                await UniTask.WaitUntil(() => _authManager.IsLoggedIn || _authManager.IsError);
                if (_authManager.IsError)
                    throw new OperationCanceledException();

                await _rankingModel.LoadMyRankingData();
                await _rankingModel.LoadRankingList();

                //自身のデータを設定
                _myRankingDataObject.SetRank(_rankingModel.MyRankingData._rank);
                _myRankingDataObject.SetUserName(_rankingModel.MyRankingData._userName);
                _myRankingDataObject.SetScore(_rankingModel.MyRankingData._score);
                _userNameInputField.text = _rankingModel.MyRankingData._userName;

                //取得数を基準に周します
                //例:3件まで表示できるが実際の取得数が2件の場合もあるため
                for (int i = 0; i < _rankingModel.RankingList.Count; i++)
                {
                    _rankingUserDataPool[i].SetRank(_rankingModel.RankingList[i]._rank);
                    _rankingUserDataPool[i].SetUserName(_rankingModel.RankingList[i]._userName);
                    _rankingUserDataPool[i].SetScore(_rankingModel.RankingList[i]._score);
                }

                _loading.CloseLoading();
                _content.SetActive(true);
            }
            catch (OperationCanceledException)
            {
                _loading.CloseLoading();
                _errorDialog.SetText("ランキングの取得に失敗しました。しばらく経ってからもう一度お試しください。");
                _errorDialog.OpenDialog();
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
        async UniTask RegisterUserData()
        {
            if (_isValidUserName == false) return;

            try
            {
                //ユーザー名とスコアを登録しランキングを更新
                await _rankingModel.RegisterUserName(_checkedUserName);
                await _rankingModel.UpdateScore(_saveDataManager.SaveData.HighScore);
                _successDialog.SetText("登録完了しました");
                await _successDialog.ShowDialogWithTimeLimit(1);
                await SetRankingDataToView();

            }
            catch (OperationCanceledException)
            {
                _errorDialog.SetText("データの登録に失敗しました。しばらく経ってからもう一度お試しください。");
                _errorDialog.OpenDialog();
            }
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