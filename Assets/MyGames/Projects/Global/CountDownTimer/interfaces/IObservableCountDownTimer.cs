using System;

namespace CountDownTimer
{
    public interface IObservableCountDownTimer
    {
        /// <summary>
        /// カウントダウンの秒数
        /// </summary>
        public int MaxCountTime { get; }
        /// <summary>
        /// カウントダウンのObservable
        /// </summary>
        public IObservable<int> CountDownObservable { get; }

        /// <summary>
        /// hot変換の準備をする
        /// </summary>
        void Publish();
        
        /// <summary>
        /// hot変換する
        /// </summary>
        void Connect();

        /// <summary>
        /// カウントの秒数を設定
        /// </summary>
        void SetCountTime(int time);

        /// <summary>
        /// カウントダウンを作成
        /// </summary>
        /// <param name="countTime"></param>
        /// <returns></returns>
        IObservable<int> CreateCountDown(int countTime);
    }

}