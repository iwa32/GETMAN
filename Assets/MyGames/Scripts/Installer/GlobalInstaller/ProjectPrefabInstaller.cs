using UnityEngine;
using Zenject;
using Fade;
using CustomSceneManager;

namespace GlobalInstaller
{
    public class ProjectPrefabInstaller : MonoInstaller
    {
        [SerializeField]
        [Header("フェードのプレハブを設定")]
        Fade.Fade _fadePrefab;

        [SerializeField]
        [Header("シーンマネージャーのプレハブを設定")]
        CustomSceneManager.CustomSceneManager _customSceneManager;

        /// <summary>
		/// シーンの切り替えを行っても破棄しないゲームオブジェクトを生成します
		/// </summary>
        public override void InstallBindings()
        {
            Container.Bind<ICustomSceneManager>()
               .FromComponentInNewPrefab(_customSceneManager)
               .AsSingle()
               .NonLazy();

            Container.Bind<IFade>()
                .FromComponentInNewPrefab(_fadePrefab)
                .AsSingle()
                .NonLazy();
        }
    }
}