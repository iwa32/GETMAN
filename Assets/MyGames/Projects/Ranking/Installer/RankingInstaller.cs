using UnityEngine;
using Zenject;
using RankingModel;

namespace RankingInstaller
{
    public class RankingInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IRankingModel>().To<RankingModelByPlayFab>().AsCached();
        }
    }
}