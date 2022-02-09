using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HpBar
{
    public class HpBar : MonoBehaviour
    {
        [SerializeField]
        [Header("Sliderを設定する")]
        Slider _slider;

        Camera _mainCamera;

        void Awake()
        {
            _mainCamera = Camera.main;
        }

        void LateUpdate()
        {
            SetRotationToCamera();
        }

        /// <summary>
        /// カメラに向きを合わせます
        /// </summary>
        void SetRotationToCamera()
        {
            transform.rotation = _mainCamera.transform.rotation;
        }

        /// <summary>
        /// 最大HPを設定する
        /// </summary>
        public void SetMaxHp(int maxHp)
        {
            _slider.maxValue = maxHp;
        }

        /// <summary>
        /// HPを設定する
        /// </summary>
        /// <param name="hp"></param>
        public void SetHp(int hp)
        {
            _slider.value = hp;
        }
    }
}
