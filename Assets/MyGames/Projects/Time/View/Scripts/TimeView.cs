using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TimeView
{
    public class TimeView : MonoBehaviour
    {
        [SerializeField]
        [Header("timer表示用テキストを設定")]
        TextMeshProUGUI _timeText;

        [SerializeField]
        [Header("警告する時間を設定を設定")]
        int _alertTime = 10;

        [SerializeField]
        [Header("警告時のテキスト色を設定")]
        Color _alertColor;

        bool _hasAlerted;

        /// <summary>
        /// timeを設定します
        /// </summary>
        /// <param name="time"></param>
        public void SetTimer(int time)
        {
            SetAlert(time);
            SetTimeText(time);
        }

        /// <summary>
        /// 警告します
        /// </summary>
        /// <param name="time"></param>
        void SetAlert(int time)
        {
            if (_hasAlerted) return;

            if (time <= _alertTime)
            {
                _timeText.color = _alertColor;
                _hasAlerted = true;
            }
        }

        /// <summary>
        /// timeをテキストに設定します
        /// </summary>
        void SetTimeText(int time)
        {
            _timeText.text = time.ToString();
        }
    }
}
