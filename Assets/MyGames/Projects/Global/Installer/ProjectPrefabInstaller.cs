using UnityEngine;
using Zenject;
using Fade;
using Dialog;
using Loading;
using AuthManager;
using CustomSceneManager;
using SaveDataManager;
using SoundManager;
using UIUtility;
using SoundModel;
using Pause;

namespace GlobalInstaller
{
    /// <summary>
    /// シーン間で共通して使用するクラスを記述する
    /// </summary>
    public class ProjectPrefabInstaller : MonoInstaller
    {
        [SerializeField]
        [Header("認証マネージャーのプレハブを設定")]
        AuthManagerByPlayFab _authManagerByPlayFab;

        [SerializeField]
        [Header("フェードのプレハブを設定")]
        Fade.Fade _fadePrefab;

        [SerializeField]
        [Header("シーンマネージャーのプレハブを設定")]
        CustomSceneManager.CustomSceneManager _customSceneManagerPrefab;

        [SerializeField]
        [Header("セーブデータマネージャーのプレハブを設定")]
        SaveDataManager.SaveDataManager _saveDataManagerPrefab;

        [SerializeField]
        [Header("サウンドマネージャーのプレハブを設定")]
        SoundManager.SoundManager _soundManagerPrefab;

        [SerializeField]
        [Header("成功時ダイアログのプレハブを設定")]
        SuccessDialog _successDialogPrefab;

        [SerializeField]
        [Header("エラーダイアログのプレハブを設定")]
        ErrorDialog _errorDialogPrefab;

        [SerializeField]
        [Header("ローディングのプレハブを設定")]
        Loading.Loading _loadingPrefab;

        /// <summary>
		/// シーンの切り替えを行っても破棄しないインスタンスを生成します
		/// </summary>
        public override void InstallBindings()
        {
            //認証
            Container.Bind<IAuthManager>()
                .FromComponentInNewPrefab(_authManagerByPlayFab)
                .AsSingle()
                .NonLazy();

            //シーン遷移
            Container.Bind<ICustomSceneManager>()
               .FromComponentInNewPrefab(_customSceneManagerPrefab)
               .AsSingle()
               .NonLazy();

            //セーブデータ管理
            Container.Bind<ISaveDataManager>()
                .FromComponentInNewPrefab(_saveDataManagerPrefab)
                .AsSingle()
                .NonLazy();

            //BGM,SE
            Container.Bind<ISoundManager>()
                .FromComponentInNewPrefab(_soundManagerPrefab)
                .AsSingle()
                .NonLazy();

            Container.Bind<ISoundModel>()
                .To<SoundModel.SoundModel>()
                .AsSingle()
                .NonLazy();

            //ダイアログ
            Container.Bind<ISuccessDialog>()
                .FromComponentInNewPrefab(_successDialogPrefab)
                .AsSingle()
                .NonLazy();

            Container.Bind<IErrorDialog>()
                .FromComponentInNewPrefab(_errorDialogPrefab)
                .AsSingle()
                .NonLazy();

            //ローディング
            Container.Bind<ILoading>()
                .FromComponentInNewPrefab(_loadingPrefab)
                .AsSingle()
                .NonLazy();

            //フェード
            Container.Bind<IFade>()
                .FromComponentInNewPrefab(_fadePrefab)
                .AsSingle()
                .NonLazy();

            //セーブデータ
            Container.Bind<ISaveData>()
                .To<SaveData>()
                .AsSingle()
                .NonLazy();

            //ポーズ
            Container.Bind<IPause>()
                .To<Pause.Pause>()
                .AsSingle()
                .NonLazy();

            //クリックボタン
            Container.Bind<IObservableClickButton>()
                .To<ObservableClickButton>()
                .AsSingle()
                .NonLazy();

            //入力フィールド
            Container.Bind<IObservableInputField>()
                .To<ObservableInputField>()
                .AsSingle()
                .NonLazy();

            //スライダー
            Container.Bind<IObservableSlider>()
                .To<ObservableSlider>()
                .AsSingle()
                .NonLazy();

            //UIの表示非表示処理
            Container.Bind<IToggleableUI>()
                .To<ToggleableUI>()
                .AsSingle()
                .NonLazy();
        }
    }
}