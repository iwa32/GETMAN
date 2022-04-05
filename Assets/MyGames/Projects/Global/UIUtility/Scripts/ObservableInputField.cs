using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UIUtility;
using TMPro;

public class ObservableInputField : IObservableInputField
{
    readonly string _maxLengthMessage = "文字以内で入力してください";

    /// <summary>
    /// 値の変更時に発火する入力イベントを作成する
    /// </summary>
    /// <param name="inputField"></param>
    /// <returns></returns>
    public IObservable<string> CreateObservableInputFieldOnValueChanged(TMP_InputField inputField)
    {
        return inputField.OnValueChangedAsObservable()
            .ThrottleFirst(TimeSpan.FromMilliseconds(500))//イベントの呼びすぎを防ぐ
            .Where(value => string.IsNullOrEmpty(value) == false);//空文字は無視
    }

    /// <summary>
    /// 値の変更後に発火する入力イベントを作成する
    /// </summary>
    /// <param name="inputField"></param>
    /// <returns></returns>
    public IObservable<string> CreateObservableInputFieldOnEndEdit(TMP_InputField inputField)
    {
        return inputField.OnEndEditAsObservable()
            .Where(value => string.IsNullOrEmpty(value) == false);//空文字は無視
    }

    /// <summary>
    /// 最大文字数チェック
    /// </summary>
    /// <param name="value"></param>
    /// <param name="max"></param>
    public (bool isValid, string message) CheckMaxLength(string value, int max)
    {
        if (value.Length <= max)
        {
            return (true, "");
        }
        return (false, max.ToString() + _maxLengthMessage);
    }
}
