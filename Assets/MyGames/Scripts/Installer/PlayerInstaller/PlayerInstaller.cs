using UnityEngine;
using Zenject;
using PlayerModel;

namespace PlayerInstaller
{
    public class PlayerInstaller : MonoInstaller
    {
        [SerializeField]
        [Header("プレイヤーの初期hpを設定")]
        int _initialHp = 3;

        [SerializeField]
        [Header("プレイヤーの初期ポイントを設定")]
        int _initialPoint = 0;

        [SerializeField]
        [Header("プレイヤーの初期スコアを設定")]
        int _initialScore = 0;

        [SerializeField]
        [Header("プレイヤーの武器の攻撃力を設定")]
        int _initialPower = 0;

        public override void InstallBindings()
        {
            Container.Bind<IHpModel>().To<HpModel>().AsCached().WithArguments(_initialHp);
            Container.Bind<IPointModel>().To<PointModel>().AsCached().WithArguments(_initialPoint);
            Container.Bind<IScoreModel>().To<ScoreModel>().AsCached().WithArguments(_initialScore);
            Container.Bind<IStateModel>().To<StateModel>().AsCached().WithArguments(PlayerState.WAIT);
            Container.Bind<IWeaponModel>().To<WeaponModel>().AsCached().WithArguments(_initialPower);
        }
    }
}