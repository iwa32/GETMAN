using UnityEngine;
using Zenject;
using PlayerModel;

namespace PlayerInstaller
{
    public class PointModelInstaller : MonoInstaller
    {
        [SerializeField]
        [Header("プレイヤーの初期ポイントを設定")]
        int _initialPoint = 0;

        public override void InstallBindings()
        {
            Container
                .Bind<IPointModel>()
                .To<PointModel>()
                .AsCached()
                .WithArguments(_initialPoint);
        }
    }
}