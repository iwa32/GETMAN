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
        SpWeaponType _type;
        #endregion

        public int Score => _score;
        public SpWeaponType Type => _type;

        public void Destroy()
        {
            gameObject.SetActive(false);
        }
    }
}