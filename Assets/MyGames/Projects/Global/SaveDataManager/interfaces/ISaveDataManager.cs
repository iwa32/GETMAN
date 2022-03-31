/// <summary>
/// セーブ管理クラス
/// </summary>
namespace SaveDataManager
{
    public interface ISaveDataManager
    {
        ISaveData SaveData { get; }
        bool IsInitialized { get; }
        string SaveCompletedMessage { get; }
        string SaveNotCompletedMessage { get; }

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
        /// 初期化フラグを設定する
        /// </summary>
        void SetIsInitialized(bool isInitialized);

        /// <summary>
        /// セーブデータが存在するか
        /// </summary>
        /// <returns></returns>
        bool SaveDataExists();

        /// <summary>
        /// データを保存します
        /// </summary>
        bool Save();

        /// <summary>
        /// データを読み込みます
        /// </summary>
        bool Load();
    }
}
