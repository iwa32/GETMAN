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
            _gameStartView.CountUntilGameStart().Forget();
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
            _gameStartView.IsGameStart
                .Where(isGameStart => isGameStart == true)
                .Subscribe(_ => { Debug.Log("動かす"); });
        }

        //ゲーム開始処理
        //ゲームオーバー処理
        //リトライ処理
        //クリア処理
    }
}
