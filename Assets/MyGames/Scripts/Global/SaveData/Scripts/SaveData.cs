using UnityEngine;
/// <summary>
/// セーブデータ保存クラス
/// </summary>
namespace SaveDataManager
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
    }
}
