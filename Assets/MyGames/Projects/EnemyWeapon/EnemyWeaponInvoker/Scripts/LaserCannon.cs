using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemyWeaponInvoker
{
    public class LaserCannon : EnemyWeaponInvoker
    {
        public override void Invoke()
        {
            Debug.Log("laser attack");
        }
    }
}