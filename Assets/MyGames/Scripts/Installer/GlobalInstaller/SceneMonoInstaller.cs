using UnityEngine;
using Zenject;
using CountDownTimer;

namespace GlobalInstaller
{
    public class SceneMonoInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            //カウントダウン
            Container.Bind<IObservableCountDownTimer>()
                .To<ObservableCountDownTimer>().AsTransient().NonLazy();
        }
    }
}