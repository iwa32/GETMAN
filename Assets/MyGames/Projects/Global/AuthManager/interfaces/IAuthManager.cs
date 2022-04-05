using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AuthManager
{
    public interface IAuthManager
    {
        public bool IsLoggedIn { get; }
        public bool IsError { get; }

        /// <summary>
        /// ログインします
        /// </summary>
        void Login();
    }
}
