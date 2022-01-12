using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using UniRx;

namespace GameView
{
    public class GameStartView : MonoBehaviour
    {
        [SerializeField]
        [Header("ゲーム開始までの秒数")]
        int _maxGameStartCount = 3;

        [SerializeField]
        [Header("カウント用のテキストを設定")]
        Text _countText;

        [SerializeField]
        [Header("ゲーム開始のアナウンス用テキストを設定")]
        Text _gameStartText;

        int _gameStartCount;
        BoolReactiveProperty _isGameStart = new BoolReactiveProperty();

        public IReadOnlyReactiveProperty<bool> IsGameStart => _isGameStart;


        public void Initialize()
        {
            _isGameStart.Value = false;
            _gameStartCount = _maxGameStartCount;
            OpenUIFor(gameObject);
            OpenUIFor(_countText.gameObject);
            CloseUIFor(_gameStartText.gameObject);
        }

        /// <summary>
        /// ゲーム開始までカウントします
        /// </summary>
        public async UniTask CountUntilGameStart()
        {
            //1秒ずつカウントします
            while (_gameStartCount > 0)
            {
                _countText.text = _gameStartCount.ToString();
                await UniTask.Delay(TimeSpan.FromSeconds(1));
                _gameStartCount--;
            }

            CloseUIFor(_countText.gameObject);
            OpenUIFor(_gameStartText.gameObject);

            await UniTask.Delay(TimeSpan.FromSeconds(1));

            CloseUIFor(_gameStartText.gameObject);
            _isGameStart.Value = true;
            CloseUIFor(gameObject);
        }

        /// <summary>
        /// UIを表示します
        /// </summary>
        /// <param name="target"></param>
        void OpenUIFor(GameObject target)
        {
            target?.SetActive(true);
        }

        /// <summary>
        /// UIを非表示にします
        /// </summary>
        /// <param name="target"></param>
        void CloseUIFor(GameObject target)
        {
            target?.SetActive(false);
        }
    }

}