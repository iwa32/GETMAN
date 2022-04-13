using System.Collections;
using System.Collections.Generic;
using StageObject;
using UnityEngine;

namespace StageObject
{
    public class HealItem : GetableItem, IHealable
    {
        [SerializeField]
        [Header("回復量を設定")]
        int _healingPower = 1;

        public int HealingPower => _healingPower;

        public override void Destroy()
        {
            gameObject.SetActive(false);
        }
    }
}