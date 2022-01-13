using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

public class CountDownTimer : MonoBehaviour
{
    int _maxCountTime;//カウントの秒数

    protected IConnectableObservable<int> _countDownObservable;

    public IObservable<int> CountDownObservable => _countDownObservable;

    /// <summary>
    /// 即時カウントダウンを開始します
    /// </summary>
    public void StartImmediateCountDown()
    {
        Publish();
        Connect();
    }

    public void Publish()
    {
        //複数のObserverに購読させるため、hot変換する
        _countDownObservable = CreateCountDown(_maxCountTime).Publish();
    }

    public void Connect()
    {
        _countDownObservable.Connect();
    }

    /// <summary>
    /// カウントの秒数を設定
    /// </summary>
    public void SetCountTime(int time)
    {
        _maxCountTime = time;
    }

    IObservable<int> CreateCountDown(int countTime)
    {
        return Observable
            .Timer(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(1))
            .Select(x => (int)(countTime - x))
            .TakeWhile(x => x > 0);//0になるまで通知を通す
    }
}