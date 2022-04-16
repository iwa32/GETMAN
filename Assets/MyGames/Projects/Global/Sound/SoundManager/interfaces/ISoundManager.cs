namespace SoundManager
{
    public interface ISoundManager
    {
        /// <summary>
        /// BGMを再生します
        /// </summary>
        /// <param name="bgmType"></param>
        void PlayBgm(BgmType bgmType);

        /// <summary>
        /// BGMを停止します
        /// </summary>
        void StopBgm();

        /// <summary>
        /// SEを再生します
        /// </summary>
        /// <param name="seType"></param>
        void PlaySE(SEType seType);
    }
}