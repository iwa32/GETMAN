using UnityEngine;

namespace UIUtility
{
    public interface IToggleableUI
    {
        /// <summary>
        /// UIを表示します
        /// </summary>
        /// <param name="target"></param>
        void OpenUIFor(GameObject target);

        /// <summary>
        /// UIを非表示にします
        /// </summary>
        /// <param name="target"></param>
        void CloseUIFor(GameObject target);
    }
}