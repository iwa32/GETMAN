using UnityEngine;
using Zenject;
using GameModel;

namespace GameInstaller
{
    public class GameInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IDirectionModel>().To<DirectionModel>().AsSingle().NonLazy();
            Container.Bind<IPointModel>().To<PointModel>().AsSingle().NonLazy();
            Container.Bind<IScoreModel>().To<ScoreModel>().AsSingle().NonLazy();
            Container.Bind<IStageNumModel>().To<StageNumModel>().AsSingle().NonLazy();
        }
    }
}