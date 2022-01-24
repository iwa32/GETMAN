using UnityEngine;
using Zenject;
using UIUtility;
using SaveData;
using CountDownTimer;

namespace GlobalInstaller
{
    public class GlobalInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            //クリックボタン
            Container.Bind<IObservableClickButton>()
                .To<ObservableClickButton>().AsSingle().NonLazy();
            //UIの表示非表示処理
            Container.Bind<IToggleableUI>()
                .To<ToggleableUI>().AsSingle().NonLazy();
            //セーブデータ
            Container.Bind<ISaveData>()
                .To<SaveData.SaveData>().AsSingle().NonLazy();
            //カウントダウン
            Container.Bind<IObservableCountDownTimer>()
                .To<ObservableCountDownTimer>().AsTransient().NonLazy();
        }
    }
}