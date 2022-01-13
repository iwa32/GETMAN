using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

public class CountDownTimer : MonoBehaviour
{
    [SerializeField]
    [Header("カウントの秒数")]
    int _maxCountTime = 3;

    protected IConnectableObservable<int> _countDownObservable;

    public IObservable<int> CountDownObservable => _countDownObservable;

    /// <summary>
    /// カウントダウンを開始します
    /// </summary>
    public void StartCountDown()
    {
        //複数のObserverに購読させるため、hot変換する
        _countDownObservable = CreateCountDown(_maxCountTime).Publish();
        _countDownObservable.Connect();
    }

    IObservable<int> CreateCountDown(int countTime)
    {
        return Observable
            .Timer(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(1))
            .Select(x => (int)(countTime - x))
            .TakeWhile(x => x > 0);//0になるまで通知を通す
    }
}