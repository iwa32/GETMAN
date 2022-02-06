using UnityEngine;
using Zenject;
using TimeModel;

namespace TimeInstaller
{
    public class TimeInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<ITimeModel>().To<TimeModel.TimeModel>().AsSingle();
        }
    }
}