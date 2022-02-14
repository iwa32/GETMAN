using Fade;
/// <summary>
/// シーン管理スクリプト
/// </summary>
namespace CustomSceneManager
{
    public interface ICustomSceneManager
    {
        IFade Fade { get; }
        void LoadScene(SceneType sceneName);
    }
}
