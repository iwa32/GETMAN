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
        [SerializeField]
        [Header("プレイヤーのPresenterを設定")]
        PlayerPresenter.PlayerPresenter _playerPresenter;

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
