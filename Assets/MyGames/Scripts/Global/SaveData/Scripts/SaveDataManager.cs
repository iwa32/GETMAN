using System.IO;
using static System.Text.Encoding;
using UnityEngine;
using Zenject;
using GameModel;

/// <summary>
/// セーブデータ保存クラス
/// </summary>
namespace SaveDataManager
{
    [System.Serializable]
    public class SaveDataManager : MonoBehaviour, ISaveDataManager
    {
        ISaveData _saveData;
        string _savePath;

        public ISaveData SaveData => _saveData;

        void Awake()
        {
            _savePath = Application.dataPath + "/MyGames/SaveData/playerData.json";
        }

        [Inject]
        public void Construct(ISaveData saveData)
        {
            _saveData = saveData;
        }

        public void SetStageNum(int stageNum)
        {
            _saveData.SetStageNum(stageNum);
        }

        public void SetScore(int score)
        {
            _saveData.SetScore(score);
        }

        public bool SaveDataExists()
        {
            return File.Exists(_savePath);
        }

        public void Save()
        {
            string jsonStr =  JsonUtility.ToJson(_saveData);

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
                    JsonUtility.FromJsonOverwrite(sr.ReadToEnd(), _saveData);
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
