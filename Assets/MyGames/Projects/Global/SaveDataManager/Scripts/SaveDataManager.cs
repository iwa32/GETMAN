using System;
using System.IO;
using static System.Text.Encoding;
using UnityEngine;
using Zenject;

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
        string _wegGlSaveKey = "SaveData";

        public ISaveData SaveData => _saveData;

        void Awake()
        {
            _savePath = Application.dataPath + "/playerData.json";
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

        /// <summary>
        /// セーブデータが存在しているか
        /// </summary>
        /// <returns></returns>
        public bool SaveDataExists()
        {
#if UNITY_EDITOR
            return File.Exists(_savePath);

#elif UNITY_WEBGL
            return (String.IsNullOrEmpty(PlayerPrefs.GetString(_wegGlSaveKey)) == false);
#endif
        }

        /// <summary>
        /// データを初期化します
        /// </summary>
        public void InitData()
        {
            if (SaveDataExists())
            {
                Save();//空データでセーブデータを上書きする
            }
        }

        /// <summary>
        /// データを保存します
        /// </summary>
        public void Save()
        {
            string jsonStr = JsonUtility.ToJson(_saveData);

#if UNITY_EDITOR
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

#elif UNITY_WEBGL
            //webGLはPlayerPrefsで保存する
            PlayerPrefs.SetString(_wegGlSaveKey, jsonStr);
#endif
        }

        /// <summary>
        /// データを読み込みます
        /// </summary>
        public void Load()
        {
#if UNITY_EDITOR
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

#elif UNITY_WEBGL
            string jsonStr = PlayerPrefs.GetString(_wegGlSaveKey);
            JsonUtility.FromJsonOverwrite(jsonStr, _saveData);
#endif
        }
    }
}
