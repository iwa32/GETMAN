using UniRx;

namespace GameModel
{
    public interface IStageNumModel
    {
        IReadOnlyReactiveProperty<int> StageNum { get; }

        /// <summary>
        /// ステージ番号の増加
        /// </summary>
        /// <param name="stageNum"></param>
        void IncreaseStageNum();

        /// <summary>
        /// ステージ番号のセット
        /// </summary>
        void SetStageNum(int stageNum);
    }
}