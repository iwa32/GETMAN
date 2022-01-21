using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SaveData
{
    public interface ISaveData
    {
        public int StageNum { get; }
        public int CurrentScore { get; }
        public int HighScore { get; }

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
        
        /// <summary>
        /// セーブデータが存在しているか
        /// </summary>
        /// <returns></returns>
        bool SaveDataExists();

        /// <summary>
        /// セーブデータの保存
        /// </summary>
        void Save();

        /// <summary>
        /// セーブデータの読み込み
        /// 
        /// </summary>
        void Load();
    }
}
