using System;
using UniRx;
using UnityEngine.UI;
using TMPro;

namespace UIUtility
{
    public interface IObservableInputField
    {
        /// <summary>
        /// 値の変更時に発火する入力イベントを作成する
        /// </summary>
        /// <param name="inputField"></param>
        /// <returns></returns>
        IObservable<string> CreateObservableInputFieldOnValueChanged(TMP_InputField inputField);

        /// <summary>
        /// 値の変更後に発火する入力イベントを作成する
        /// </summary>
        /// <param name="inputField"></param>
        /// <returns></returns>
        IObservable<string> CreateObservableInputFieldOnEndEdit(TMP_InputField inputField);

        /// <summary>
        /// 最大文字数チェック
        /// </summary>
        /// <param name="value"></param>
        /// <param name="max"></param>
        (bool isValid, string message) CheckMaxLength(string value, int max);
    }
}