using UnityEngine;
using Zenject;
using UIUtility;

namespace GlobalInstaller
{
    public class GlobalInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IObservableClickButton>()
                .To<ObservableClickButton>().AsSingle();
        }
    }
}