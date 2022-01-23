using UnityEngine;
using Zenject;
using UIUtility;
using SaveData;

namespace GlobalInstaller
{
    public class GlobalInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IObservableClickButton>()
                .To<ObservableClickButton>().AsSingle().NonLazy();

            Container.Bind<ISaveData>()
                .To<SaveData.SaveData>().AsSingle().NonLazy();
        }
    }
}