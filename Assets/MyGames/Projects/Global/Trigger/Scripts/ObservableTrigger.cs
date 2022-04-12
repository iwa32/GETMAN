using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx.Triggers;
using System;

namespace Trigger
{
    public class ObservableTrigger : MonoBehaviour
    {
        /// <summary>
        /// 接触時
        /// </summary>
        /// <returns></returns>
        public IObservable<Collider> OnTriggerEnter()
        {
            return this.OnTriggerEnterAsObservable();
        }

        /// <summary>
        /// 接触中
        /// </summary>
        /// <returns></returns>
        public IObservable<Collider> OnTriggerStay()
        {
            return this.OnTriggerStayAsObservable();
        }

        /// <summary>
        /// 接触後
        /// </summary>
        /// <returns></returns>
        public IObservable<Collider> OnTriggerExit()
        {
            return this.OnTriggerExitAsObservable();
        }
    }
}