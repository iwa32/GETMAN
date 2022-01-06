using UnityEngine;
using Zenject;
using PlayerModel;

namespace PlayerInstaller
{
    public class WeaponModelInstaller : MonoInstaller
    {
        [SerializeField]
        [Header("プレイヤーの武器の攻撃力を設定")]
        int _initialPower = 0;

        public override void InstallBindings()
        {
            Container
                .Bind<IWeaponModel>()
                .To<WeaponModel>()
                .AsCached()
                .WithArguments(_initialPower);
        }
    }
}