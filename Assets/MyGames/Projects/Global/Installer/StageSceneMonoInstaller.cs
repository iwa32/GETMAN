using UnityEngine;
using Zenject;
using CountDownTimer;
using ObjectPool;

namespace GlobalInstaller
{
    /// <summary>
    /// ステージシーンで使用する汎用クラスのDI設定
    /// </summary>
    public class StageSceneMonoInstaller : MonoInstaller
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

            //ポイントアイテム
            Container.Bind<IPointItemPool>()
                .To<PointItemPool>().AsSingle().NonLazy();

            //SP武器
            Container.Bind<ISpPlayerWeaponPool>()
                .To<SpWeaponPool>().AsSingle().NonLazy();
        }
    }
}