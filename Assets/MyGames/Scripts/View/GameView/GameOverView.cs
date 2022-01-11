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
    public class GameOverView : MonoBehaviour
    {
        [SerializeField]
        [Header("リトライボタンを設定")]
        Button _retryButton;

        [SerializeField]
        [Header("タイトルボタンを設定")]
        Button _toTitleButton;
    }
}