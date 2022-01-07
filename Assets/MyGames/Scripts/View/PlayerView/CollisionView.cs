using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx.Triggers;

public class CollisionView : MonoBehaviour
{
    public IObservable<Collision> OnCollision()
    {
        return this.OnCollisionEnterAsObservable();
    }
}
