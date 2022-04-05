using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UIUtility;
using TMPro;

public class ObservableInputField : IObservableInputField
{
    /// <summary>
    /// 値の変更時に発火する入力イベントを作成する
    /// </summary>
    /// <param name="inputField"></param>
    /// <returns></returns>
    public IObservable<string> CreateObservableInputFieldOnValueChanged(TMP_InputField inputField)
    {
        return inputField.OnValueChangedAsObservable()
            .ThrottleFirst(TimeSpan.FromMilliseconds(1000));//イベントの呼び出し間隔を制御
    }

    /// <summary>
    /// 値の変更後に発火する入力イベントを作成する
    /// </summary>
    /// <param name="inputField"></param>
    /// <returns></returns>
    public IObservable<string> CreateObservableInputFieldOnEndEdit(TMP_InputField inputField)
    {
        return inputField.OnEndEditAsObservable();
    }
}
