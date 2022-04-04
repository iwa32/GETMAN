using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RankingModel;
using Zenject;
using SoundManager;
using UIUtility;
using UnityEngine.UI;
using UniRx;
using RankingView;
using Cysharp.Threading.Tasks;

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
        [Header("ランキングデータのプレハブを設定")]
        RankingUserDataView _rankingUserDataPrefab;

        [SerializeField]
        [Header("ランキングデータの表示領域を設定")]
        GameObject _content;

        [SerializeField]
        [Header("ランキングの表示数を設定")]
        int _maxResultCount = 3;

        #region//フィールド
        IRankingModel _rankingModel;
        IToggleableUI _toggleableUI;
        ISoundManager _soundManager;
        IObservableClickButton _observableClickButton;
        RankingUserDataView[] _rankingUserDataPool;//ランキングviewを保管しておく
        #endregion

        [Inject]
        public void Construct(
            IRankingModel rankingModel,
            IToggleableUI toggleableUI,
            ISoundManager soundManager,
            IObservableClickButton observableClickButton
        )
        {
            _rankingModel = rankingModel;
            _toggleableUI = toggleableUI;
            _soundManager = soundManager;
            _observableClickButton = observableClickButton;
        }

        // Start is called before the first frame update
        void Start()
        {
            //CloseRanking();
            _rankingModel.Login();//todo 別クラスで行う
            Initialize();
            Bind();
            //uniRxでbottonをクリックで必要なメソッドを呼び出す
        }

        private void Initialize()
        {
            //取得数の設定 取得数は定数にするか迷い中
            _rankingModel.SetMaxResultCount(_maxResultCount);
            PreCreateRankingDataView();
        }

        void Bind()
        {
            _observableClickButton.
                CreateObservableClickButton(_closeRankingButton)
                .Subscribe(_ => CloseRanking())
                .AddTo(this);

            //todo 
            //inputField
            //登録ボタンを押した時

            _observableClickButton.
                CreateObservableClickButton(_registerRankingButton)
                .Subscribe(_ => RegisterUserData())
                .AddTo(this);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("クリック");
                SetRankingDataToView().Forget();
            }
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
            await _rankingModel.LoadRankingList();

            //実際に取得したランキングデータの数だけ描画します
            //例:3件まで取得できるが実際の取得数が2件の場合もあるため
            for (int i = 0; i < _rankingModel.RankingList.Count; i++)
            {
                _rankingUserDataPool[i].SetRank(_rankingModel.RankingList[i]._rank);
                _rankingUserDataPool[i].SetUserName(_rankingModel.RankingList[i]._userName);
                _rankingUserDataPool[i].SetScore(_rankingModel.RankingList[i]._score);
            }

            _content.SetActive(true);
        }

        void RegisterUserData()
        {
            //入力内容を取得
            //バリデーション
            //文字数および、先頭と末尾の空白削除
            //問題なければ登録、あればエラーメッセージを出す
            _rankingModel.RegisterUserName("presenter");
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