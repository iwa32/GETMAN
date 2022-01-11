using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using UniRx;
using static GameViewStrings;

namespace GameView
{
    public class GameStartView : MonoBehaviour
    {
        [SerializeField]
        [Header("ゲーム開始までの秒数")]
        int _gameStartCount = 3;

        [SerializeField]
        [Header("カウント用のテキストを設定")]
        Text _countText;

        [SerializeField]
        [Header("ゲーム開始のアナウンス用テキストを設定")]
        Text _gameStartText;

        public IReadOnlyReactiveProperty<bool> IsGameStart => _isGameStart;

        BoolReactiveProperty _isGameStart = new BoolReactiveProperty();

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

            _countText.text = "";
            _gameStartText.text = GAME_START;

            await UniTask.Delay(TimeSpan.FromSeconds(1));

            _gameStartText.text = "";
            _isGameStart.Value = true;
        }
    }

}