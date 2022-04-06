using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using Dialog;
using Zenject;
using System.Text;

namespace AuthManager
{
    public class AuthManagerByPlayFab : MonoBehaviour, IAuthManager
    {
        readonly string _customIdKey = "CustomId";
        readonly string _idCharacters
            = "0123456789abcdefghijklmnopqrstuvwxyz";//IDに使用する文字

        bool _isLoggedIn;
        bool _isError;
        string _customId;//ログインid todo saveDataManagerと統合するかは検討中
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
            _customId = GetCustomId();

            PlayFabClientAPI.LoginWithCustomID(
                new LoginWithCustomIDRequest
                {
                    CustomId = _customId,
                    CreateAccount = true
                },
                result => OnSuccess(result),
                error => OnError()
                );
        }

        void OnSuccess(LoginResult result)
        {
            //作成時にidを保存する
            if (result.NewlyCreated)
                SaveCustomId();

            _isLoggedIn = true;
            Debug.Log("ログイン成功");
        }

        void OnError()
        {
            _errorDialog.SetText("ログインに失敗しました。しばらく経ってからもう一度お試しください。");
            _errorDialog.OpenDialog();
            _isError = true;
        }

        void SaveCustomId()
        {
            PlayerPrefs.SetString(_customIdKey, _customId);
        }

        /// <summary>
        /// PlayFabIdを取得します
        /// </summary>
        /// <returns></returns>
        string GetCustomId()
        {
            string id = PlayerPrefs.GetString(_customIdKey);

            //なければ生成します
            if (string.IsNullOrEmpty(id))
            {
                id = GenerateCustomId();
            }

            return id;
        }

        /// <summary>
        /// PlayFabIdを生成する
        /// 参考URL:https://kan-kikuchi.hatenablog.com/entry/PlayFabLogin#%E3%83%AD%E3%82%B0%E3%82%A4%E3%83%B3%E5%87%A6%E7%90%86
        /// </summary>
        /// <returns></returns>
        string GenerateCustomId()
        {
            int idLength = 32;//IDの長さ
            StringBuilder stringBuilder = new StringBuilder(idLength);
            var random = new System.Random();

            //ランダムにIDを生成
            for (int i = 0; i < idLength; i++)
            {
                stringBuilder.Append(_idCharacters[random.Next(_idCharacters.Length)]);
            }

            return stringBuilder.ToString();
        }
    }
}
