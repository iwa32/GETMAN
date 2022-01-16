using UnityEngine;
using Zenject;
using EnemyModel;

namespace EnemyInstaller
{
    public class EnemyInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IHpModel>().To<HpModel>().AsTransient();
            Container.Bind<IPowerModel>().To<PowerModel>().AsTransient();
            Container.Bind<IScoreModel>().To<ScoreModel>().AsTransient();
        }
    }
}