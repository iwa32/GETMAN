using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
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
using Loading;
using Pause;
using static SceneType;
using static SEType;
using UIUtility;
using PlayerView;

namespace GamePresenter
{
    public class GamePresenter : MonoBehaviour
    {
        readonly string _gameClearMessage = "ステージクリアおめでとうございます！<br>次のステージ追加をお待ちください。";
        readonly string _loadFailureMessage = "ゲームの読み込みに失敗しました。<br>再読み込みをお試しください。";

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
        [Header("ポーズボタンのUIを設定")]
        PauseView _pauseView;

        [SerializeField]
        [Header("プレイヤーのPresenterのプレハブを設定")]
        PlayerPresenter.PlayerPresenter _playerPrefab;

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

        [SerializeField]
        [Header("HPのUIを設定")]
        HpView _hpView;

        [SerializeField]
        [Header("SP武器表示用のUIを設定")]
        SpWeaponView _spWeaponView;

        [SerializeField]
        [Header("cinemachineのバーチャルカメラを設定")]
        CinemachineVirtualCamera _cinemachineVirtualCamera;
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
        IErrorDialog _errorDialog;
        ILoading _loading;
        IPause _pause;
        IObservableClickButton _observableClickButton;
        PlayerPresenter.PlayerPresenter _playerPresenter;
        #endregion

        [Inject]
        DiContainer container;//動的生成したデータにDIできるようにする

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
            ISuccessDialog successDialog,
            IErrorDialog errorDialog,
            ILoading loading,
            IPause pause,
            IObservableClickButton observableClickButton
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
            _errorDialog = errorDialog;
            _loading = loading;
            _pause = pause;
            _observableClickButton = observableClickButton;
        }

        void Awake()
        {
            _timePresenter.ManualAwake();
            _stagePresenter.ManualAwake();
        }

        void Start()
        {
            InitializeAsync().Forget();
        }

        async UniTask InitializeAsync()
        {
            try
            {
                _loading.OpenLoading();
                //データの読み込み、初期化
                _pointModel.SetPoint(_initialPoint);
                await LoadGameData();
                await _stagePresenter.InitializeAsync();
                await SetUpPlayer();
                _playerPresenter.PlaceOnStage(_stagePresenter.GetPlayerStartingTransform());
                _timePresenter.Initialize(_stagePresenter.StageLimitCountTime);
                _scoreView.SetScore(_scoreModel.Score.Value);
                _stageNumView.SetStageNum(_stageNumModel.StageNum.Value);
                //演出
                await _fade.StartFadeIn();
                _loading.CloseLoading();
                await _stagePresenter.PlaceBossEnemyToStage();
                _gameStartView.Initialize();
                _gameStartView.StartCount();
                Bind();
            }
            catch(OperationCanceledException)
            {
                _loading.CloseLoading();
                _errorDialog.SetText(_loadFailureMessage);
                _errorDialog.OpenDialog();
            }
        }

        /// <summary>
        /// プレイヤーの準備をします
        /// </summary>
        /// <returns></returns>
        async UniTask SetUpPlayer()
        {
            await CreatePlayer();
            _cinemachineVirtualCamera.Follow = _playerPresenter.transform;
            _cinemachineVirtualCamera.LookAt = _playerPresenter.transform;
        }

        /// <summary>
        /// プレイヤーを作成します
        /// </summary>
        /// <returns></returns>
        async UniTask CreatePlayer()
        {
            _playerPresenter = container.InstantiatePrefab(_playerPrefab)
                .GetComponent<PlayerPresenter.PlayerPresenter>();
            _playerPresenter.SetPlayerUI(_hpView, _spWeaponView);

            await UniTask.WaitUntil(() => _playerPresenter != null);
        }

        void FixedUpdate()
        {
            if (_directionModel.IsGameStart.Value == false) return;
            if (_playerPresenter == null) return;
            _playerPresenter.ManualFixedUpdate();
        }

        /// <summary>
        /// modelとviewの監視、処理
        /// </summary>
        void Bind()
        {
            //view
            //カウントダウン後、ゲーム開始処理を行います
            _gameStartView.IsGameStart
                .Where(isGameStart => isGameStart == true)
                .Subscribe(_ => StartGame())
                .AddTo(this);

            //停止ボタン
            //ポーズ中は停止ボタンのストリームを購読できないため、破棄し再生成します
            Action pauseButtonAction = () => _pause.ChangePause(!_pause.IsPause.Value);

            _observableClickButton.RepeatObserveButtonForPause(
                _pauseView.PauseButtonAsObservable,
                pauseButtonAction
            );
            
            _pause.IsPause
                .Subscribe(isPause => {
                    _directionModel.SetIsGamePause(isPause);
                    _pauseView.TogglePauseIcon(isPause);
                })
                .AddTo(this);

            //プレイヤーのボタン入力
            //リスタートボタン
            _gameOverView.ClickRestartButton()
                .Subscribe(_ => RestartGame().Forget())
                .AddTo(this);
            //次のステージへ
            _gameClearView.ClickNextStageButton()
                .Subscribe(_ => LoadNextStage().Forget())
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
        async UniTask RestartGame()
        {
            //スコアをリセット
            _scoreModel.SetScore(_initialScore);
            await SaveGameData(false);
            _soundManager.PlaySE(SCENE_MOVEMENT);
            _customSceneManager.LoadScene(STAGE);
        }

        /// <summary>
        /// 次のステージを読み込みます
        /// </summary>
        async UniTask LoadNextStage()
        {
            //次のステージが存在する場合
            int nextStageNum = _stagePresenter.GetNextStageNum();
            if (_stagePresenter.CheckStage(nextStageNum))
            {
                _stageNumModel.SetStageNum(nextStageNum);
                await SaveGameData(false);
                _soundManager.PlaySE(SCENE_MOVEMENT);
                _customSceneManager.LoadScene(STAGE);
            }
            else
            {
                _successDialog.SetText(_gameClearMessage);
                _successDialog.OpenDialog();
                await UniTask.Yield();
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
        }

        /// <summary>
        /// ゲームクリア
        /// </summary>
        void GameClear()
        {
            _soundManager.PlaySE(GAME_CLEAR);
            _gameClearView.gameObject?.SetActive(true);

            SaveGameData(false).Forget();
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

            await UniTask.WaitUntil(() => isSaved);


            //ダイアログを表示する場合
            if (isShownDialog == false) return;
            if (isSaved)
                _successDialog.SetText(_saveDataManager.SaveCompletedMessage);
            else
                _successDialog.SetText(_saveDataManager.SaveNotCompletedMessage);

            await _successDialog.ShowDialogWithTimeLimit(1);
        }

        /// <summary>
        /// ゲームデータを読み込む
        /// </summary>
        async UniTask LoadGameData()
        {
            int score = _initialScore;
            int stageNum = _initialStageNum;

            //最初から始める
            if (_saveDataManager.IsInitialized)
            {
                _saveDataManager.SetIsInitialized(false);
            }
            else
            {
                score = _saveDataManager.SaveData.CurrentScore;
                stageNum = _saveDataManager.SaveData.StageNum;
            }

            _scoreModel.SetScore(score);
            _stageNumModel.SetStageNum(stageNum);
            await UniTask.Yield();
        }
    }
}
