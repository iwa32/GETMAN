using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerView
{
    public class SpWeaponView : MonoBehaviour
    {
        [SerializeField]
        [Header("SP武器のアイコン表示用UIを設定")]
        Image _spWeaponImage;

        /// <summary>
        /// sp武器のアイコンを設定します
        /// </summary>
        /// <param name="icon"></param>
        public void SetIcon(Sprite icon)
        {
            _spWeaponImage.sprite = icon;
        }
    }
}
