using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx.Triggers;

namespace TriggerView
{
    public class CollisionView : MonoBehaviour
    {
        /// <summary>
        /// 衝突時
        /// </summary>
        /// <returns></returns>
        public IObservable<Collision> OnCollisionEnter()
        {
            return this.OnCollisionEnterAsObservable();
        }

        /// <summary>
        /// 衝突中
        /// </summary>
        /// <returns></returns>
        public IObservable<Collision> OnCollisionStay()
        {
            return this.OnCollisionStayAsObservable();
        }

        /// <summary>
        /// 衝突後
        /// </summary>
        /// <returns></returns>
        public IObservable<Collision> OnCollisionExit()
        {
            return this.OnCollisionExitAsObservable();
        }
    }
}
