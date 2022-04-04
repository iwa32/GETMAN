using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace RankingModel
{
    public interface IRankingModel
    {
        public List<UserData> RankingList { get; }
        public int MaxResultsCount { get; }

        /// <summary>
        /// ランキングへの最大表示数を設定します
        /// </summary>
        /// <param name="max"></param>
        void SetMaxResultCount(int max);

        /// <summary>
        /// ユーザー名を登録
        /// </summary>
        /// <param name="userName"></param>
        void RegisterUserName(string userName);

        /// <summary>
        /// ランキング情報を取得
        /// </summary>
        UniTask LoadRankingList();

        /// <summary>
        /// スコアの更新
        /// </summary>
        /// <param name="score"></param>
        void UpdateScore(int score);
    }
}