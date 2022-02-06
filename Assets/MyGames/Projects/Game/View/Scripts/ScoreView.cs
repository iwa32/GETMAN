using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GameView
{
    public class ScoreView : MonoBehaviour
    {
        [SerializeField]
        [Header("スコア表示用テキストを設定")]
        TextMeshProUGUI scoreText;
    
        /// <summary>
        /// Scoreの更新
        /// </summary>
        public void SetScore(int score)
        {
            scoreText.text = score.ToString();
        }
    }
}
