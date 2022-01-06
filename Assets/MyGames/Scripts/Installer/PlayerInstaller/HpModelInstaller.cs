using UnityEngine;
using Zenject;
using PlayerModel;

namespace PlayerInstaller
{
    public class HpModelInstaller : MonoInstaller
    {
        [SerializeField]
        [Header("プレイヤーの初期hpを設定")]
        int _initialHp = 3;

        public override void InstallBindings()
        {
            Container
                .Bind<IHpModel>()
                .To<HpModel>()
                .AsCached()
                .WithArguments(_initialHp);
        }
    }
}