using CustomSceneManager;
using UnityEngine;
using Zenject;

namespace GlobalInstaller
{
    public class NotDestroyableObjectInstaller : MonoInstaller
    {
        /// <summary>
		/// シーンの切り替えを行っても破棄しないゲームオブジェクトを生成します
		/// </summary>
        public override void InstallBindings()
        {
            Container.Bind<ICustomSceneManager>()
                .To<CustomSceneManager.CustomSceneManager>()
                .FromNewComponentOnNewGameObject()
                .AsSingle()
                .NonLazy();
        }
    }
}