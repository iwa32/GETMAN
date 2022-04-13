using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalInterface;

namespace StageObject
{
    public class StageTrap : MonoBehaviour, IPlayerAttacker
    {
        #region//インスペクターで設定
        [SerializeField]
        [Header("与えるダメージを設定")]
        int _power;
        #endregion

        #region//プロパティ
        public int Power => _power;
        #endregion
    }
}