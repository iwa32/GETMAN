using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using Cysharp.Threading.Tasks;

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
        List<UserData> _rankingList = new List<UserData>();
        int _maxResultsCount;
        readonly string _rankingStatisticName = "HighScore";


        public List<UserData> RankingList => _rankingList;
        public int MaxResultsCount => _maxResultsCount;

        public void Login()
        {
            PlayFabClientAPI.LoginWithCustomID(
                new LoginWithCustomIDRequest
                {
                    CustomId = "ChanceID",
                    CreateAccount = true
                },
                result => Debug.Log("ログイン成功"),
                error => Debug.Log("ログイン失敗")
                );
        }

        public void SetMaxResultCount(int max)
        {
            _maxResultsCount = max;
        }

        public void RegisterUserName(string userName)
        {
            UpdateUserTitleDisplayNameRequest request
                = new UpdateUserTitleDisplayNameRequest
                {
                    DisplayName = userName
                };

            PlayFabClientAPI.UpdateUserTitleDisplayName(
                request,
                result => Debug.Log(string.Format("名前登録完了{0}", request.DisplayName)),
                error => Debug.Log("名前登録できない")
            );
        }

        public async UniTask LoadRankingList()
        {
            //todo ログイン状態になるまでまつ

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
                ////初期化する
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
                isLoaded = true;
            },
            error => Debug.Log(error.GenerateErrorReport())
            );//todo エラーでunitaskを中断

            //データを格納するまで待ちます
            await UniTask.WaitUntil(() => isLoaded);
        }

        public void UpdateScore(int score)
        {
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
                result => Debug.Log("スコア送信"),
                error => Debug.Log(error.GenerateErrorReport())
                );
        }
    }
}
