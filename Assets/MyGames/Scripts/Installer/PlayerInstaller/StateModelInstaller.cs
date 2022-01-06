using UnityEngine;
using Zenject;
using PlayerModel;

namespace PlayerInstaller
{
    public class StateModelInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container
                .Bind<IStateModel>()
                .To<StateModel>()
                .AsCached()
                .WithArguments(PlayerState.WAIT);
        }
    }
}