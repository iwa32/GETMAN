using UniRx;
using System;

namespace CountDownTimer
{
    public class ObservableCountDownTimer: IObservableCountDownTimer
    {
        int _maxCountTime;//カウントの秒数

        protected IConnectableObservable<int> _countDownObservable;

        public int MaxCountTime => _maxCountTime;
        public IObservable<int> CountDownObservable => _countDownObservable;

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

        public IObservable<int> CreateCountDown(int countTime)
        {
            return Observable
                .Timer(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(1))
                .Select(x => (int)(countTime - x))
                .TakeWhile(x => x > 0);//0になるまで通知を通す
        }
    }
}