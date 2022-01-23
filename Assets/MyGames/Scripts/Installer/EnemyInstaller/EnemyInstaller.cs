using UnityEngine;
using Zenject;
using EnemyModel;

namespace EnemyInstaller
{
    public class EnemyInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IHpModel>().To<HpModel>().AsTransient().NonLazy();
            Container.Bind<IPowerModel>().To<PowerModel>().AsTransient().NonLazy();
            Container.Bind<IScoreModel>().To<ScoreModel>().AsTransient().NonLazy();
        }
    }
}