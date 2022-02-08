using System.Collections;
using System.Collections.Generic;
using Strategy;
using UnityEngine;
using UniRx;

namespace StrategyView
{
    /// <summary>
    /// 追跡処理を実装する
    /// </summary>
    public abstract class TrackStrategy : MonoBehaviour, IStrategy
    {
        #region//インスペクターから設定
        [SerializeField]
        [Header("追跡エリアのコンポーネントを設定")]
        protected TrackingAreaView.TrackingAreaView _trackingAreaView;

        public IReactiveProperty<bool> CanTrack => _trackingAreaView.CanTrack;
        #endregion

        public abstract void Strategy();
    }
}
