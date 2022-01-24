using UnityEngine;
using Zenject;
using PlayerModel;

namespace PlayerInstaller
{
    public class PlayerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IHpModel>().To<HpModel>().AsCached();
            Container.Bind<IWeaponModel>().To<WeaponModel>().AsCached();
        }
    }
}