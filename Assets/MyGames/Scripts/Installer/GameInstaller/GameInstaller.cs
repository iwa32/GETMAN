using UnityEngine;
using Zenject;
using GameModel;

namespace GameInstaller
{
    public class GameInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IDirectionModel>().To<DirectionModel>().AsSingle();
            Container.Bind<IPointModel>().To<PointModel>().AsSingle();
            Container.Bind<IScoreModel>().To<ScoreModel>().AsSingle();
            Container.Bind<IStageNumModel>().To<StageNumModel>().AsSingle();
        }
    }
}