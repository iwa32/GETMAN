using UnityEngine;
using Zenject;
using CountDownTimer;
using ObjectPool;

namespace GlobalInstaller
{
    public class SceneMonoInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            //カウントダウン
            Container.Bind<IObservableCountDownTimer>()
                .To<ObservableCountDownTimer>().AsTransient().NonLazy();

            //オブジェクトプール
            //エネミー
            Container.Bind<IEnemyPool>()
                .To<EnemyPool>().AsSingle().NonLazy();
        }
    }
}