using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StageObject
{
    public class SpWeaponItem : GetableItem,
    ISpWeaponItem
    {
        #region//インスペクターで設定
        [SerializeField]
        [Header("SP武器の種類を設定")]
        SpWeaponType _type;
        #endregion

        public SpWeaponType Type => _type;


        public override void Destroy()
        {
            gameObject.SetActive(false);
        }
    }
}