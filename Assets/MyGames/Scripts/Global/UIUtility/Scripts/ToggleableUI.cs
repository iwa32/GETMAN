using UnityEngine;

namespace UIUtility
{
    public class ToggleableUI : IToggleableUI
    {
        public void OpenUIFor(GameObject target)
        {
            target?.SetActive(true);
        }

        public void CloseUIFor(GameObject target)
        {
            target?.SetActive(false);
        }
    }
}
