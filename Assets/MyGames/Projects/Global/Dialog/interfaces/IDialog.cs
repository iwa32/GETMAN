using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dialog
{
    public interface IDialog
    {
        /// <summary>
        /// テキストに使用する文字列を設定します
        /// </summary>
        /// <param name="text"></param>
        void SetText(string text);

        /// <summary>
        /// ダイアログを開きます
        /// </summary>
        void OpenDialog();

        /// <summary>
        /// ダイアログを閉じます
        /// </summary>
        void CloseDialog();
    }
}
