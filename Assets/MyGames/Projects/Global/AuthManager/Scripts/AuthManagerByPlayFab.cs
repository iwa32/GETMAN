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
        public bool IsLoggedIn => _isLoggedIn;

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
                result => Debug.Log("ログイン成功"),
                error => Debug.Log("ログイン失敗")
                );
        }
    }
}
