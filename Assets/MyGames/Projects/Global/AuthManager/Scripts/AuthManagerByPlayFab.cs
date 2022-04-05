using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

namespace AuthManager
{
    public class AuthManagerByPlayFab : MonoBehaviour, IAuthManager
    {
        bool _isLoggedIn;
        bool _isError;

        public bool IsLoggedIn => _isLoggedIn;
        public bool IsError => _isError;

        void Awake()
        {
            Login();
        }

        public void Login()
        {
            PlayFabClientAPI.LoginWithCustomID(
                new LoginWithCustomIDRequest
                {
                    CustomId = "ChanceID",
                    CreateAccount = true
                },
                result => OnSuccess(),
                error => OnError()
                );
        }

        void OnSuccess()
        {
            _isLoggedIn = true;
            Debug.Log("ログイン成功");
        }

        void OnError()
        {
            _isError = true;
            Debug.Log("ログイン失敗");
            //ダイアログを出した後falseにする
        }
    }
}
