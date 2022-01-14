using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace StateView
{
    public abstract class StateView : MonoBehaviour
    {
        public Action DelAction { get; set; }

        public abstract StateType State { get; set; }

        public void Action()
        {
            DelAction();
        }
    }
}
