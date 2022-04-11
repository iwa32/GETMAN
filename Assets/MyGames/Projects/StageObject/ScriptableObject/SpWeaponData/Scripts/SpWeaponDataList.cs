using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        SpWeaponType _type;

        public SpWeaponType Type => _type;

        //GameObjectをIplayerWeaponとして提供する
    }
}
