using UnityEngine;
using Zenject;
using PlayerModel;

namespace PlayerInstaller
{
    public class ScoreModelInstaller : MonoInstaller
    {
        [SerializeField]
        [Header("プレイヤーの初期スコアを設定")]
        int _initialScore = 0;

        public override void InstallBindings()
        {
            Container
                .Bind<IScoreModel>()
                .To<ScoreModel>()
                .AsCached()
                .WithArguments(_initialScore);
        }
    }
}