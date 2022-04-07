using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIUtility;
using Zenject;

namespace Loading
{
    public class Loading : MonoBehaviour, ILoading
    {
        IToggleableUI _toggleableUI;

        void Start()
        {
            Initialize();
        }

        void Initialize()
        {
            _toggleableUI.CloseUIFor(gameObject);
        }

        [Inject]
        public void Construct(
            IToggleableUI toggleableUI
        )
        {
            _toggleableUI = toggleableUI;
        }

        public void OpenLoading()
        {
            _toggleableUI.OpenUIFor(gameObject);
        }

        public void CloseLoading()
        {
            _toggleableUI.CloseUIFor(gameObject);
        }
    }
}