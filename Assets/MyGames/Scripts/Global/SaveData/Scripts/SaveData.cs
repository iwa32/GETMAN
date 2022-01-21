using System.IO;
using static System.Text.Encoding;
using UnityEngine;
/// <summary>
/// セーブデータ保存クラス
/// </summary>
namespace SaveData
{
    [System.Serializable]
    public class SaveData : ISaveData
    {
        [SerializeField]
        int _stageNum;
        [SerializeField]
        int _highScore;
        [SerializeField]
        int _currentScore;

        string _savePath = Application.dataPath + "/MyGames/SaveData/playerData.json";
        //float _bgmVolume;
        //float _seVolume;

        public int StageNum => _stageNum;
        public int CurrentScore => _currentScore;
        public int HighScore => _highScore;
        

        public void SetStageNum(int stageNum)
        {
            _stageNum = stageNum;
        }

        public void SetScore(int score)
        {
            //ハイスコアを更新
            if (score > _highScore)
                _highScore = score;

            _currentScore = score;
        }

        public bool SaveDataExists()
        {
            return File.Exists(_savePath);
        }

        public void Save()
        {
            string jsonStr =  JsonUtility.ToJson(this);

            //ファイルに出力
            using (StreamWriter sw = new StreamWriter(_savePath, false, UTF8))
            {
                try
                {
                    sw.Write(jsonStr);
                    sw.Flush();
                    sw.Close();//念の為明記
                }
                catch
                {
                    Debug.Log("データを保存できませんでした。");
                }
            }
        }

        public void Load()
        {
            using (StreamReader sr = new StreamReader(_savePath))
            {
                try
                {
                    JsonUtility.FromJsonOverwrite(sr.ReadToEnd(), this);
                    sr.Close();
                }
                catch
                {
                    Debug.Log("データを読み込めませんでした。");
                }
            }
        }
    }
}
