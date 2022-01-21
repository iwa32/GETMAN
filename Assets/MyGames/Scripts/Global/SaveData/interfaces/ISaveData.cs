using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SaveData
{
    public interface ISaveData
    {
        public int StageNum { get; }
        public int HighScore { get; }

        /// <summary>
        /// ステージ番号を設定
        /// </summary>
        /// <param name="stageNum"></param>
        public void SetStageNum(int stageNum);

        /// <summary>
        /// ハイスコアを設定
        /// </summary>
        /// <param name="highScore"></param>
        public void SetHighScore(int highScore);

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
