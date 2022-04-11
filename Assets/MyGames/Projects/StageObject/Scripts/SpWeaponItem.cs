using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StageObject
{
    public class SpWeaponItem : MonoBehaviour,
    ISpWeaponItem
    {
        #region//インスペクターで設定
        [SerializeField]
        [Header("獲得スコア")]
        int _score;

        [SerializeField]
        [Header("SP武器の種類を設定")]
        SpWeaponType _spWeaponType;
        #endregion

        public int Score => _score;
        public SpWeaponType SpWeaponType => _spWeaponType;

        public void Destroy()
        {
            Debug.Log("destroy");
        }
    }
}