using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Zenject;
using GameModel;

namespace GamePresenter
{
    public class GamePresenter : MonoBehaviour
    {
        #region//インスペクターから設定
        [SerializeField]
        [Header("プレイヤーのPresenterを設定")]
        PlayerPresenter.PlayerPresenter _playerPresenter;
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
            _playerPresenter.Initialize();
        }

        void FixedUpdate()
        {
            _playerPresenter.ManualFixedUpdate();
        }
    }
}
