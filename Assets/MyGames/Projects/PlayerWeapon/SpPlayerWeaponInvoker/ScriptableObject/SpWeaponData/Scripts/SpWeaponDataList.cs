using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpPlayerWeaponInvoker;

namespace SpWeaponDataList
{
    [CreateAssetMenu(fileName = "SpWeaponDataList", menuName = "ScriptableObject/Create SpWeaponDataList")]
    public class SpWeaponDataList : ScriptableObject
    {
        [SerializeField]
        List<SpWeaponData> _spWeaponDataList = new List<SpWeaponData>();

        public List<SpWeaponData> GetSpWeaponDataList => _spWeaponDataList;

        /// <summary>
        /// SP武器を取得します
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public SpWeaponData FindSpWeaponDataByType(SpWeaponType type)
        {
            try
            {
                return _spWeaponDataList.Find(spWeapon => spWeapon.Type == type);
            }
            catch
            {
                Debug.Log("Sp武器が見つかりませんでした");
                return null;
            }
        }
    }

    [System.Serializable]
    public class SpWeaponData
    {
        [SerializeField]
        [Tooltip("SP武器の種類を設定")]
        SpWeaponType _type = SpWeaponType.NONE;

        [SerializeField]
        [Tooltip("UI表示用のスプライトを設定")]
        Sprite _uiIcon;

        [SerializeField]
        [Header("攻撃力を設定")]
        int _power = 1;

        [SerializeField]
        [Tooltip("SP武器発動用プレハブを設定する")]
        GameObject _spWeaponInvoker;


        public SpWeaponType Type => _type;
        public Sprite UIIcon => _uiIcon;
        public int Power => _power;
        public ISpPlayerWeaponInvoker SpWeaponInvoker
        {
            get { return _spWeaponInvoker?.GetComponent<ISpPlayerWeaponInvoker>(); }
        }
    }
}
