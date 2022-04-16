using UniRx;

namespace SoundModel
{
    public interface ISoundModel
    {
        public IReadOnlyReactiveProperty<float> BgmVolume { get; }
        public IReadOnlyReactiveProperty<float> SEVolume { get; }
        public IReadOnlyReactiveProperty<bool> BgmIsMute { get; }
        public IReadOnlyReactiveProperty<bool> SEIsMute { get; }

        void SetBgmVolume(float volume);

        void SetSEVolume(float volume);

        void SetBgmIsMute(bool isMute);

        void SetSEIsMute(bool isMute);
    }
}
