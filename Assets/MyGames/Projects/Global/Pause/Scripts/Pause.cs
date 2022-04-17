using UniRx;
using UnityEngine;

namespace Pause
{
    public class Pause : IPause
    {
        BoolReactiveProperty _isPause = new BoolReactiveProperty();

        public IReactiveProperty<bool> IsPause => _isPause;

        public void ChangePause(bool isPause)
        {
            if (isPause)
                DoPause();
            else
                Resume();
        }

        public void DoPause()
        {
            if (_isPause.Value) return;

            Time.timeScale = 0;
            _isPause.Value = true;
        }

        public void Resume()
        {
            if (_isPause.Value == false) return;

            Time.timeScale = 1;
            _isPause.Value = false;
        }
    }
}
