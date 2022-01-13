using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using Zenject;
using GameModel;
using GameView;

namespace GamePresenter
{
    public class GamePresenter : MonoBehaviour
    {
        #region//インスペクターから設定
        [SerializeField]
        [Header("プレイヤーのPresenterを設定")]
        PlayerPresenter.PlayerPresenter _playerPresenter;

        [SerializeField]
        [Header("ゲーム開始UIを設定")]
        GameStartView _gameStartView;

        [SerializeField]
        [Header("ゲームオーバーUIを設定")]
        GameOverView _gameOverView;
        #endregion

        #region//フィールド
        IGameModel _gameModel;
        #endregion

        [Inject]
        public void Construct(IGameModel gameModel)
        {
            _gameModel = gameModel;
        }

        void Awake()
        {
            _playerPresenter.ManualAwake();
        }

        void Start()
        {
            _gameStartView.Initialize();
            _playerPresenter.Initialize();
            Bind();
        }

        void FixedUpdate()
        {
            _playerPresenter.ManualFixedUpdate();
        }

        /// <summary>
        /// modelとviewの監視、処理
        /// </summary>
        void Bind()
        {
            //view
            _gameStartView.IsGameStart
                .Where(isGameStart => isGameStart == true)
                .Subscribe(_ => StartGame())
                .AddTo(this);

            _playerPresenter.IsGameOver
                .Where(isGameOver => isGameOver == true)
                .Subscribe(_ => _gameModel.SetIsGameOver(true));

            //コンティニューボタン
            _gameOverView.ClickContinueButton()
                .Subscribe(_ => _gameModel.SetIsGameContinue(true));

            //タイトルボタン
            _gameOverView.ClickToTitleButton()
                .Subscribe(_ => Debug.Log("ToTitle"));

            //model
            _gameModel.IsGameOver
                .Where(isGameOver => isGameOver == true)
                .Subscribe(_ => GameOver());
            _gameModel.IsGameContinue
                .Where(isGameContinue => isGameContinue == true)
                .Subscribe(_ => ContinueGame());
        }

        /// <summary>
        /// ゲームを開始する
        /// </summary>
        void StartGame()
        {
            _playerPresenter.SetCanStartGame(true);
        }

        /// <summary>
        /// ゲームをコンティニューする
        /// </summary>
        void ContinueGame()
        {
            _gameOverView.gameObject?.SetActive(false);
            //todo フェードを出現させる
            _playerPresenter.ResetData();
            _gameStartView.Initialize();
        }

        /// <summary>
        /// ゲームオーバー
        /// </summary>
        void GameOver()
        {
            _gameOverView.gameObject?.SetActive(true);
        }

        //クリア処理
    }
}
