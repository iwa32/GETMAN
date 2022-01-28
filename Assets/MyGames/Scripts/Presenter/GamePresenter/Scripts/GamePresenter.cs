using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using Zenject;
using GameModel;
using GameView;
using SaveData;
using CustomSceneManager;
using static SceneType;

namespace GamePresenter
{
    public class GamePresenter : MonoBehaviour
    {
        #region//インスペクターから設定
        [SerializeField]
        [Header("初期ポイントを設定")]
        int _initialPoint = 0;

        [SerializeField]
        [Header("初期スコアを設定")]
        int _initialScore = 0;

        [SerializeField]
        [Header("初期ステージ番号を設定")]
        int _initialStageNum = 1;

        [SerializeField]
        [Header("Hpを取得するスコアライン")]
        int _scoreLineToGetHp = 100;

        [SerializeField]
        [Header("次は〇倍後のスコアラインでHpを取得します")]
        int _nextMagnification = 5;

        [SerializeField]
        [Header("スコアのUIを設定")]
        ScoreView _scoreView;

        [SerializeField]
        [Header("獲得ポイントのUIを設定")]
        PointView _pointView;

        [SerializeField]
        [Header("ステージ番号のUIを設定")]
        StageNumView _stageNumView;

        [SerializeField]
        [Header("プレイヤーのPresenterを設定")]
        PlayerPresenter.PlayerPresenter _playerPresenter;

        [SerializeField]
        [Header("TimerのPresenterを設定")]
        TimePresenter.TimePresenter _timePresenter;

        [SerializeField]
        [Header("StageのPresenterを設定")]
        StagePresenter.StagePresenter _stagePresenter;

        [SerializeField]
        [Header("ゲーム開始UIを設定")]
        GameStartView _gameStartView;

        [SerializeField]
        [Header("ゲームオーバーUIを設定")]
        GameOverView _gameOverView;

        [SerializeField]
        [Header("ゲームクリアUIを設定")]
        GameClearView _gameClearView;
        #endregion

        #region//フィールド
        IDirectionModel _directionModel;
        IScoreModel _scoreModel;
        IPointModel _pointModel;
        IStageNumModel _stageNumModel;
        ISaveData _saveData;
        ICustomSceneManager _customSceneManager;
        #endregion

        [Inject]
        public void Construct(
            IDirectionModel directionModel,
            IScoreModel score,
            IPointModel point,
            IStageNumModel stageNum,
            ISaveData saveData,
            ICustomSceneManager customSceneManager
        )
        {
            _directionModel = directionModel;
            _scoreModel = score;
            _pointModel = point;
            _stageNumModel = stageNum;
            _saveData = saveData;
            _customSceneManager = customSceneManager;
        }

        void Awake()
        {
            _playerPresenter.ManualAwake();
            _timePresenter.ManualAwake();
            _stagePresenter.ManualAwake();
        }

        void Start()
        {
            LoadGameData();
            _pointModel.SetPoint(_initialPoint);
            _gameStartView.Initialize();
            _playerPresenter.Initialize();
            _timePresenter.Initialize();
            _stagePresenter.Initialize();
            Bind();
        }

        void FixedUpdate()
        {
            if (_directionModel.IsGameStart.Value == false) return;
            _playerPresenter.ManualFixedUpdate();
        }

        /// <summary>
        /// modelとviewの監視、処理
        /// </summary>
        void Bind()
        {
            //view
            _gameStartView.IsGameStart
                .Where(isGameStart => isGameStart == true)
                .Subscribe(_ => StartGame())
                .AddTo(this);

            //コンティニューボタン
            _gameOverView.ClickContinueButton()
                .Subscribe(_ => ContinueGame())
                .AddTo(this);

            _gameClearView.ClickNextStageButton()
                .Subscribe(_ => LoadNextStage())
                .AddTo(this);

            //タイトルボタン
            _gameOverView.ClickToTitleButton()
                .Subscribe(_ => _customSceneManager.LoadScene(TITLE))
                .AddTo(this);

            _gameClearView.ClickToTitleButton()
                .Subscribe(_ => _customSceneManager.LoadScene(TITLE))
                .AddTo(this);

            //ステージ
            //生成
            _stagePresenter.IsCreatedState
                .Where(isCreatedStage => isCreatedStage == true)
                .Subscribe(_ =>
                _stagePresenter.PlacePlayerToStage(_playerPresenter.transform)
                );

            //プレイヤーの配置
            _stagePresenter.IsPlacedPlayer
                .Where(isPlacedPlayer => isPlacedPlayer == true)
                .Subscribe(_ => _gameStartView.StartCount())
                .AddTo(this);

            //model
            _directionModel.IsGameOver
                .Where(isGameOver => isGameOver == true)
                .Subscribe(_ => GameOver())
                .AddTo(this);

            //todo 不要？
            //_directionModel.IsGameContinue
            //    .Where(isGameContinue => isGameContinue == true)
            //    .Subscribe(_ => ContinueGame())
            //    .AddTo(this);

            _directionModel.IsGameClear
                .Where(isGameClear => isGameClear == true)
                .Subscribe(_ => GameClear())
                .AddTo(this);

            _scoreModel.Score
                .Subscribe(score => CheckScore(score))
                .AddTo(this);

            _pointModel.Point
                .Subscribe(point => _pointView.SetPointGauge(point))
                .AddTo(this);

            _stageNumModel.StageNum
                .Subscribe(stageNum => _stageNumView.SetStageNum(stageNum))
                .AddTo(this);
        }

        /// <summary>
        /// スコアを監視する
        /// </summary>
        void CheckScore(int score)
        {
            CheckScoreToGetHp(score);
            _scoreView.SetScore(score);
        }

        /// <summary>
        /// Scoreを決められた数取得するとHPがアップします
        /// </summary>
        /// <param name="score"></param>
        void CheckScoreToGetHp(int score)
        {
            if (score <= 0) return;//scoreが0の時はアップしない
            if (score % _scoreLineToGetHp == 0)
            {
                //playerのHpを1つ増やす
                _playerPresenter.AddHp(1);
                _scoreLineToGetHp *= _nextMagnification;
            }
        }

        /// <summary>
        /// ゲームを開始する
        /// </summary>
        void StartGame()
        {
            _directionModel.SetIsGameStart(true);
        }

        /// <summary>
        /// ゲームをコンティニューする
        /// </summary>
        void ContinueGame()
        {
            //シーンの再読み込みをする
            _customSceneManager.LoadScene(STAGE);
        }

        /// <summary>
        /// 次のステージを読み込みます
        /// </summary>
        void LoadNextStage()
        {
            //次のステージが存在する場合
            int nextStageNum = _stageNumModel.StageNum.Value + 1;
            if (_stagePresenter.CheckStage(nextStageNum))
            {
                _saveData.SetStageNum(nextStageNum);
                _saveData.Save();
                _customSceneManager.LoadScene(STAGE);
            }
            else
            {
                //todo ダイアログUIを表示する 
                Debug.Log("ステージが見つかりませんでした");
            }
        }

        /// <summary>
        /// ゲームオーバー
        /// </summary>
        void GameOver()
        {
            SaveGameData(true);
            _gameOverView.gameObject?.SetActive(true);
            _playerPresenter.ChangeDead();
        }

        /// <summary>
        /// ゲームクリア
        /// </summary>
        void GameClear()
        {
            SaveGameData(false);
            _gameClearView.gameObject?.SetActive(true);
            _playerPresenter.ChangeJoy();
        }

        /// <summary>
        /// ゲームデータを保存する
        /// </summary>
        void SaveGameData(bool isReset)
        {
            int score = _scoreModel.Score.Value;
            int stageNum = _stageNumModel.StageNum.Value;

            //現在のスコアを初期値にします
            if (isReset)
            {
                score = _initialScore;
                stageNum = _initialStageNum;
            }

            //ステージ番号、スコアを保存
            _saveData.SetScore(score);
            _saveData.SetStageNum(stageNum);
            _saveData.Save();
        }

        /// <summary>
        /// ゲームデータを読み込む
        /// </summary>
        void LoadGameData()
        {
            int score = _initialScore;
            int stageNum = _initialStageNum;

            //saveDataがあればそちらを取得し設定する
            if (_saveData.SaveDataExists())
            {
                _saveData.Load();
                score = _saveData.CurrentScore;
                stageNum = _saveData.StageNum;
            }

            _scoreModel.SetScore(score);
            _stageNumModel.SetStageNum(stageNum);
        }
    }
}
