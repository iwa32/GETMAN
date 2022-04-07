using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using Zenject;
using GameModel;
using GameView;
using SaveDataManager;
using CustomSceneManager;
using SoundManager;
using Fade;
using Dialog;
using static SceneType;
using static SEType;

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
        [Header("ゲームクリア後、次のステージがない場合に表示するメッセージを設定します")]
        string _gameClearMessage = "ステージクリアおめでとうございます！次のステージ追加をお待ちください。";

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
        ISaveDataManager _saveDataManager;
        ICustomSceneManager _customSceneManager;
        ISoundManager _soundManager;
        IFade _fade;
        ISuccessDialog _successDialog;
        #endregion

        [Inject]
        public void Construct(
            IDirectionModel direction,
            IScoreModel score,
            IPointModel point,
            IStageNumModel stageNum,
            ISaveDataManager saveDataManager,
            ICustomSceneManager customSceneManager,
            ISoundManager soundManager,
            IFade fade,
            ISuccessDialog successDialog
        )
        {
            _directionModel = direction;
            _scoreModel = score;
            _pointModel = point;
            _stageNumModel = stageNum;
            _saveDataManager = saveDataManager;
            _customSceneManager = customSceneManager;
            _soundManager = soundManager;
            _fade = fade;
            _successDialog = successDialog;
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
            //ゲームの準備
            //ステージの生成後、プレイヤーを配置します
            _stagePresenter.IsCreatedState
                .Where(isCreatedStage => isCreatedStage == true)
                .Subscribe(_ =>
                _stagePresenter.PlacePlayerToStage(_playerPresenter.transform)
                );

            //プレイヤーの配置が完了後、フェードインとゲーム開始の準備のカウントを行う
            _stagePresenter.IsPlacedPlayer
                .Where(isPlacedPlayer => isPlacedPlayer == true)
                .Subscribe(_ => _fade.FadeInBeforeAction(_gameStartView.StartCount).Forget())
                .AddTo(this);

            //view
            //カウントダウン後、ゲーム開始処理を行います
            _gameStartView.IsGameStart
                .Where(isGameStart => isGameStart == true)
                .Subscribe(_ => StartGame())
                .AddTo(this);

            //プレイヤーのボタン入力
            //リスタートボタン
            _gameOverView.ClickRestartButton()
                .Subscribe(_ => RestartGame())
                .AddTo(this);
            //次のステージへ
            _gameClearView.ClickNextStageButton()
                .Subscribe(_ => LoadNextStage())
                .AddTo(this);

            //タイトルボタン
            _gameOverView.ClickToTitleButton()
                .Subscribe(_ => MoveToTitle())
                .AddTo(this);

            _gameClearView.ClickToTitleButton()
                .Subscribe(_ => MoveToTitle())
                .AddTo(this);
            
            //model
            //ゲームの進行
            _directionModel.IsGameOver
                .Where(isGameOver => isGameOver == true)
                .Subscribe(_ => GameOver())
                .AddTo(this);

            _directionModel.IsGameClear
                .Where(isGameClear => isGameClear == true)
                .Subscribe(_ => GameClear())
                .AddTo(this);

            //ゲームデータの監視
            _scoreModel.Score
                .Subscribe(score => CheckScore(score))
                .AddTo(this);

            _pointModel.Point
                .Subscribe(point => _pointView.SetPointGauge(point))
                .AddTo(this);

            _stageNumModel.StageNum
                .First()
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
        /// ゲームを再スタートする
        /// </summary>
        void RestartGame()
        {
            _saveDataManager.SetIsInitialized(true);
            _soundManager.PlaySE(SCENE_MOVEMENT);
            //シーンの再読み込みをする
            _customSceneManager.LoadScene(STAGE);
        }

        /// <summary>
        /// 次のステージを読み込みます
        /// </summary>
        void LoadNextStage()
        {
            //次のステージが存在する場合
            int nextStageNum = _stagePresenter.GetNextStageNum();
            if (_stagePresenter.CheckStage(nextStageNum))
            {
                _stageNumModel.SetStageNum(nextStageNum);
                SaveGameData(false).Forget();
                _soundManager.PlaySE(SCENE_MOVEMENT);
                _customSceneManager.LoadScene(STAGE);
            }
            else
            {
                _successDialog.SetText(_gameClearMessage);
                _successDialog.OpenDialog();
            }
        }

        /// <summary>
        /// タイトルへ移動します
        /// </summary>
        void MoveToTitle()
        {
            _soundManager.PlaySE(SCENE_MOVEMENT);
            _customSceneManager.LoadScene(TITLE);
        }

        /// <summary>
        /// ゲームオーバー
        /// </summary>
        void GameOver()
        {
            _soundManager.PlaySE(GAME_OVER);
            _gameOverView.gameObject?.SetActive(true);
            SaveGameData(false).Forget();
            _playerPresenter.ChangeDead();
        }

        /// <summary>
        /// ゲームクリア
        /// </summary>
        void GameClear()
        {
            _soundManager.PlaySE(GAME_CLEAR);
            _gameClearView.gameObject?.SetActive(true);

            SaveGameData(false).Forget();
            _playerPresenter.ChangeJoy();
        }

        /// <summary>
        /// ゲームデータを保存する
        /// </summary>
        async UniTask SaveGameData(bool isShownDialog)
        {
            await UniTask.Yield();//最後に獲得したスコアが反映されない場合があるため、1フレ待ちます
            //ステージ番号、スコアを保存
            _saveDataManager.SetScore(_scoreModel.Score.Value);
            _saveDataManager.SetStageNum(_stageNumModel.StageNum.Value);
            
            bool isSaved = _saveDataManager.Save();

            if (isShownDialog == false) return;
            //ダイアログを表示する場合
            if (isSaved)
                _successDialog.SetText(_saveDataManager.SaveCompletedMessage);
            else
                _successDialog.SetText(_saveDataManager.SaveNotCompletedMessage);

            await _successDialog.ShowDialogWithTimeLimit(1);
        }

        /// <summary>
        /// ゲームデータを読み込む
        /// </summary>
        void LoadGameData()
        {
            int score = _initialScore;
            int stageNum = _initialStageNum;

            //最初から始める
            if (_saveDataManager.IsInitialized)
            {
                _scoreModel.SetScore(score);
                _stageNumModel.SetStageNum(stageNum);
                _saveDataManager.SetIsInitialized(false);
                return;
            }

            //次のステージへ行く場合のみ使用する
            if (_saveDataManager.IsLoaded)
            {
                score = _saveDataManager.SaveData.CurrentScore;
                stageNum = _saveDataManager.SaveData.StageNum;
            }

            _scoreModel.SetScore(score);
            _stageNumModel.SetStageNum(stageNum);
        }
    }
}
