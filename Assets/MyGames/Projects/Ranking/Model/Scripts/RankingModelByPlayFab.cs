using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

namespace RankingModel
{
    //todo自身と他のプレイヤーで共通して使い回したいので後で別の場所に移動する
    public struct UserData
    {
        public string _id;
        public string _userName;
        public int _rank;
        public int _score;
    }

    public class RankingModelByPlayFab : IRankingModel
    {
        readonly string _rankingStatisticName = "HighScore";

        #region//フィールド
        List<UserData> _rankingList = new List<UserData>();
        int _maxResultsCount;
        CancellationTokenSource _cts = new CancellationTokenSource();
        #endregion

        #region//プロパティ
        public List<UserData> RankingList => _rankingList;
        public int MaxResultsCount => _maxResultsCount;
        #endregion

        public void SetMaxResultCount(int max)
        {
            _maxResultsCount = max;
        }

        /// <summary>
        /// ユーザー名の登録
        /// </summary>
        /// <param name="userName"></param>
        public async UniTask RegisterUserName(string userName)
        {
            CancellationToken token = _cts.Token;
            bool isRegistered = false;

            UpdateUserTitleDisplayNameRequest request
                = new UpdateUserTitleDisplayNameRequest
                {
                    DisplayName = userName
                };

            PlayFabClientAPI.UpdateUserTitleDisplayName(
                request,
                result => isRegistered = true,
                error => _cts.Cancel()
            );

            await UniTask.WaitUntil(() => isRegistered, cancellationToken: token);
        }

        /// <summary>
        /// ランキングの読み込み
        /// </summary>
        /// <returns></returns>
        public async UniTask LoadRankingList()
        {
            CancellationToken token = _cts.Token;
            bool isLoaded = false;

            GetLeaderboardRequest request
                = new GetLeaderboardRequest
                {
                    StatisticName = _rankingStatisticName,
                    StartPosition = 0,
                    MaxResultsCount = _maxResultsCount
                };

            PlayFabClientAPI.GetLeaderboard(
            request,
            result =>
            {
                SetRankingList(result);
                isLoaded = true;
            },
            error => { _cts.Cancel(); }
            );

            //データを格納するまで待ちます
            await UniTask.WaitUntil(() => isLoaded, cancellationToken: token);
        }

        void SetRankingList(GetLeaderboardResult result)
        {
            //初期化する
            _rankingList.Clear();
            result.Leaderboard
            .ForEach(
                x =>
                {
                    _rankingList.Add(
                        new UserData
                        {
                            _id = x.PlayFabId,
                            _userName = x.DisplayName,
                            _rank = x.Position + 1,//ランキングは1から開始する
                            _score = x.StatValue
                        }
                     );
                }
            );
        }

        /// <summary>
        /// スコアの更新
        /// </summary>
        /// <param name="score"></param>
        public async UniTask UpdateScore(int score)
        {
            CancellationToken token = _cts.Token;
            bool isUpdated = false;

            UpdatePlayerStatisticsRequest request
                = new UpdatePlayerStatisticsRequest
                {
                    Statistics = new List<StatisticUpdate>()
                    {
                        new StatisticUpdate
                        {
                            StatisticName = _rankingStatisticName,
                            Value = score
                        }
                    }
                };

            PlayFabClientAPI.UpdatePlayerStatistics(
                request,
                result => isUpdated = true,
                error => _cts.Cancel()
                );

            await UniTask.WaitUntil(() => isUpdated, cancellationToken: token);
        }
    }
}
