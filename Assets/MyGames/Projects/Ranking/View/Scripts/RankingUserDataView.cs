using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace RankingView
{
    public class RankingUserDataView : MonoBehaviour
    {
        [SerializeField]
        [Header("ランク表示用テキスト")]
        TextMeshProUGUI _rank;

        [SerializeField]
        [Header("ユーザー名表示用テキスト")]
        TextMeshProUGUI _userName;

        [SerializeField]
        [Header("ハイスコア表示用テキスト")]
        TextMeshProUGUI _score;

        public void SetRank(int rank)
        {
            _rank.text = rank.ToString();
        }

        public void SetUserName(string userName)
        {
            _userName.text = userName;
        }

        public void SetScore(int score)
        {
            _score.text = score.ToString();
        }
    }
}
