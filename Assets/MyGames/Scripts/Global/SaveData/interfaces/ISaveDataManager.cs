/// <summary>
/// セーブ管理クラス
/// </summary>
namespace SaveDataManager
{
    public interface ISaveDataManager
    {
        ISaveData SaveData { get; }

        /// <summary>
        /// ステージ番号を設定する
        /// </summary>
        /// <param name="stageNum"></param>
        void SetStageNum(int stageNum);

        /// <summary>
        /// スコアを設定する
        /// </summary>
        /// <param name="score"></param>
        void SetScore(int score);

        /// <summary>
        /// セーブデータが存在するか
        /// </summary>
        /// <returns></returns>
        bool SaveDataExists();

        /// <summary>
        /// データを保存します
        /// </summary>
        void Save();

        /// <summary>
        /// データを読み込みます
        /// </summary>
        void Load();
    }
}
