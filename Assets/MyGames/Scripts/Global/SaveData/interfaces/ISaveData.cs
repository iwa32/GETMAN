/// <summary>
/// セーブデータ保存クラス
/// </summary>
namespace SaveDataManager
{
    public interface ISaveData
    {
        int StageNum { get; }
        int CurrentScore { get; }
        int HighScore { get; }

        /// <summary>
        /// ステージ番号を設定
        /// </summary>
        /// <param name="stageNum"></param>
        void SetStageNum(int stageNum);

        /// <summary>
        /// スコアを設定
        /// </summary>
        /// <param name="score"></param>
        void SetScore(int score);
    }
}
