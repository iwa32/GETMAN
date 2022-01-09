using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace PlayerView
{
    public abstract class StateView : MonoBehaviour
    {
        public Action DelAction { get; set; }

        public abstract PlayerState State { get; set; }

        public void Action()
        {
            DelAction();
        }
    }
}
