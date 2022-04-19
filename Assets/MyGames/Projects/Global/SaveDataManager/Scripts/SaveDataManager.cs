using System;
using System.IO;
using static System.Text.Encoding;
using UnityEngine;
using Zenject;
using Dialog;

/// <summary>
/// セーブデータ保存クラス
/// </summary>
namespace SaveDataManager
{
    [System.Serializable]
    public class SaveDataManager : MonoBehaviour, ISaveDataManager
    {
        [SerializeField]
        [Header("セーブ完了後に表示するメッセージを設定します")]
        string _saveCompletedMessage = "データを保存しました";
        [SerializeField]
        [Header("セーブができなかった場合に表示するメッセージを設定します")]
        string _saveNotCompletedMessage = "データが保存できませんでした、再度お試しください";

        string _savePath;
        bool _isInitialized;
        bool _isLoaded;
        ISaveData _saveData;
        IErrorDialog _errorDialog;
#if UNITY_WEBGL
        string _wegGlSaveKey = "SaveData";
#endif

        public ISaveData SaveData => _saveData;
        public bool IsInitialized => _isInitialized;
        public bool IsLoaded => _isLoaded;
        public string SaveCompletedMessage => _saveCompletedMessage;
        public string SaveNotCompletedMessage => _saveNotCompletedMessage;

        void Awake()
        {
            _savePath = Application.dataPath + "/playerData.json";
            Load();
        }

        [Inject]
        public void Construct(
            ISaveData saveData,
            IErrorDialog errorDialog
        )
        {
            _saveData = saveData;
            _errorDialog = errorDialog;
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
        /// 初期化フラグを設定する
        /// </summary>
        public void SetIsInitialized(bool isInitialized)
        {
            _isInitialized = isInitialized;
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
        /// データを保存します
        /// </summary>
        public bool Save()
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
                    return true;
                }
                catch
                {
                    _errorDialog.SetText("データを保存できませんでした。もう一度お試しください。");
                    _errorDialog.OpenDialog();
                    return false;
                }
            }

#elif UNITY_WEBGL
            //webGLはPlayerPrefsで保存する
            PlayerPrefs.SetString(_wegGlSaveKey, jsonStr);
            return true;
#endif
        }

        /// <summary>
        /// データを読み込みます
        /// </summary>
        public void Load()
        {
            if (SaveDataExists() == false)
            {
                _isLoaded = true;
                return;
            }

#if UNITY_EDITOR
            using (StreamReader sr = new StreamReader(_savePath))
            {
                try
                {
                    JsonUtility.FromJsonOverwrite(sr.ReadToEnd(), _saveData);
                    sr.Close();
                    _isLoaded = true;
                }
                catch
                {
                    _errorDialog.SetText("データを読み込めませんでした。もう一度お試しください。");
                    _errorDialog.OpenDialog();
                    _isLoaded = false;
                }
            }

#elif UNITY_WEBGL
            string jsonStr = PlayerPrefs.GetString(_wegGlSaveKey);
            JsonUtility.FromJsonOverwrite(jsonStr, _saveData);
            _isLoaded = true;
#endif
        }
    }
}
