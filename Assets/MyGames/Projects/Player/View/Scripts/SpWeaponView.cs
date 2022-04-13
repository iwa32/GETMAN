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

        int OPACITY_ZERO = 0;
        int OPACITY_MAX = 255;

        void Awake()
        {
            SetIconColorAlpha(OPACITY_ZERO);//最初はアイコンを透明にします。
        }

        /// <summary>
        /// sp武器のアイコンを設定します
        /// </summary>
        /// <param name="icon"></param>
        public void SetIcon(Sprite icon)
        {
            //透明なら見えるようにする
            if (_spWeaponImage.color.a != OPACITY_MAX)
                SetIconColorAlpha(OPACITY_MAX);
            
            _spWeaponImage.sprite = icon;
        }

        void SetIconColorAlpha(int alphaValue)
        {
            Color color = _spWeaponImage.color;
            color.a = alphaValue;
            _spWeaponImage.color = color;
        }
    }
}
