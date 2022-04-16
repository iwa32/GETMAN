using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UIUtility;

namespace SoundView
{
    public class SoundView : MonoBehaviour
    {
        [SerializeField]
        [Header("サウンドオプションのキャンバスを設定します")]
        Canvas _soundOptionCanvas;

        #region//フィールド
        IToggleableUI _toggleableUI;
        #endregion

        [Inject]
        public void Construct(
            IToggleableUI toggleableUI
        )
        {
            _toggleableUI = toggleableUI;
        }

        public void OpenSoundOption()
        {
            _toggleableUI.OpenUIFor(_soundOptionCanvas?.gameObject);
        }

        public void CloseSoundOption()
        {
            _toggleableUI.CloseUIFor(_soundOptionCanvas?.gameObject);
        }

        /// <summary>
        /// bgmのミュートアイコンを切り替えます
        /// </summary>
        /// <param name="isMute"></param>
        public void ToggleBgmMuteIcon(bool isMute)
        {
            Debug.Log("togglebgm" + isMute);
        }

        public void ToggleSEMuteIcon(bool isMute)
        {
            Debug.Log("togglese" + isMute);
        }
    }
}
