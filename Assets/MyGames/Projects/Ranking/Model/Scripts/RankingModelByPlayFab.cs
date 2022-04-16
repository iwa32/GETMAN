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
    public struct UserData
    {
        public string _id;
        public string _userName;
        public int _rank;
        public int _score;
    }

    public class RankingModelByPlayFab : IRankingModel
    {
        const int PLAYFAB_SYSTEM_ERROR_STRING_LENGTH = 2;

        readonly string _rankingStatisticName = "HighScore";

        #region//フィールド
        List<UserData> _rankingList = new List<UserData>();
        UserData _myRankingData = new UserData();
        int _maxResultsCount;
        #endregion

        #region//プロパティ
        public List<UserData> RankingList => _rankingList;
        public UserData MyRankingData => _myRankingData;
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
            //2文字以下の場合エラーにため、空白をつけてから登録します
            if (userName.Length <= PLAYFAB_SYSTEM_ERROR_STRING_LENGTH)
            {
                userName += "  ";
            }

            CancellationTokenSource cts = new CancellationTokenSource();
            CancellationToken token = cts.Token;
            bool isRegistered = false;

            UpdateUserTitleDisplayNameRequest request
                = new UpdateUserTitleDisplayNameRequest
                {
                    DisplayName = userName
                };

            PlayFabClientAPI.UpdateUserTitleDisplayName(
                request,
                result => isRegistered = true,
                error => cts.Cancel()
            );

            await UniTask.WaitUntil(() => isRegistered, cancellationToken: token);
        }


        /// <summary>
        /// 自身のランキングデータを取得する
        /// </summary>
        /// <returns></returns>
        public async UniTask LoadMyRankingData()
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            CancellationToken token = cts.Token;
            bool isLoaded = false;

            GetLeaderboardAroundPlayerRequest request =
                new GetLeaderboardAroundPlayerRequest
                {
                    StatisticName = _rankingStatisticName,
                    MaxResultsCount = 1//自身のみを取得するので1件指定する
                };

            PlayFabClientAPI.GetLeaderboardAroundPlayer(
            request,
            result =>
            {
                _myRankingData._id = result.Leaderboard[0].PlayFabId;
                _myRankingData._userName = result.Leaderboard[0].DisplayName;
                _myRankingData._rank = result.Leaderboard[0].Position + 1;//ランキングは1から開始するため
                _myRankingData._score = result.Leaderboard[0].StatValue;
                isLoaded = true;
            },
            error => { cts.Cancel(); }
            );

            //データを格納するまで待ちます
            await UniTask.WaitUntil(() => isLoaded, cancellationToken: token);
        }

        /// <summary>
        /// ランキングの読み込み
        /// </summary>
        /// <returns></returns>
        public async UniTask LoadRankingList()
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            CancellationToken token = cts.Token;
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
            error => { cts.Cancel(); }
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
            CancellationTokenSource cts = new CancellationTokenSource();
            CancellationToken token = cts.Token;
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
                error =>
                {
                    Debug.Log(error.GenerateErrorReport());
                    cts.Cancel();
                }
                );

            await UniTask.WaitUntil(() => isUpdated, cancellationToken: token);
        }
    }
}
