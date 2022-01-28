using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StageObject
{
    public class StageTrap : MonoBehaviour, IDamager
    {
        #region//インスペクターで設定
        [SerializeField]
        [Header("与えるダメージを設定")]
        int _damage;
        #endregion

        #region//プロパティ
        public int Damage => _damage;
        #endregion
    }
}