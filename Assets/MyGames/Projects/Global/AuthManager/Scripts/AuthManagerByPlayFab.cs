using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using Dialog;
using Zenject;

namespace AuthManager
{
    public class AuthManagerByPlayFab : MonoBehaviour, IAuthManager
    {
        bool _isLoggedIn;
        bool _isError;
        IErrorDialog _errorDialog;

        public bool IsLoggedIn => _isLoggedIn;
        public bool IsError => _isError;


        void Start()
        {
            Login();
        }

        [Inject]
        public void Construct(
            IErrorDialog errorDialog
        )
        {
            _errorDialog = errorDialog;
        }

        public void Login()
        {
            PlayFabClientAPI.LoginWithCustomID(
                new LoginWithCustomIDRequest
                {
                    CustomId = "ChanceID",//todo
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
            _errorDialog.SetText("ログインに失敗しました。しばらく経ってからもう一度お試しください。");
            _errorDialog.OpenDialog();
            _isError = true;
        }
    }
}
