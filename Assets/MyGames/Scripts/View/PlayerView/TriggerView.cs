using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx.Triggers;
using System;

namespace PlayerView
{
    public class TriggerView : MonoBehaviour
    {
        public IObservable<Collider> OnTrigger()
        {
            return this.OnTriggerEnterAsObservable();
        }
    }
}