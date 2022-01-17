using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using Zenject;
using GameModel;
using GameView;

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
        int nextMagnification = 5;

        [SerializeField]
        [Header("スコアのUIを設定")]
        ScoreView _scoreView;

        [SerializeField]
        [Header("獲得ポイントのUIを設定")]
        PointView _pointView;

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
        #endregion

        #region//フィールド
        IDirectionModel _directionModel;
        IScoreModel _scoreModel;
        IPointModel _pointModel;
        IStageNumModel _stageNumModel;
        #endregion

        [Inject]
        public void Construct(
            IDirectionModel directionModel,
            IScoreModel score,
            IPointModel point,
            IStageNumModel stageNum
        )
        {
            _directionModel = directionModel;
            _scoreModel = score;
            _pointModel = point;
            _stageNumModel = stageNum;
        }

        void Awake()
        {
            _playerPresenter.ManualAwake();
            _timePresenter.ManualAwake();
            _stagePresenter.ManualAwake();
        }

        void Start()
        {
            _scoreModel.SetScore(_initialScore);
            _pointModel.SetPoint(_initialPoint);
            _stageNumModel.SetStageNum(_initialStageNum);
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
                .Subscribe(_ => _directionModel.SetIsGameContinue(true))
                .AddTo(this);

            //タイトルボタン
            _gameOverView.ClickToTitleButton()
                .Subscribe(_ => Debug.Log("ToTitle"))
                .AddTo(this);

            //model
            _directionModel.IsGameOver
                .Where(isGameOver => isGameOver == true)
                .Subscribe(_ => GameOver())
                .AddTo(this);

            _directionModel.IsGameContinue
                .Where(isGameContinue => isGameContinue == true)
                .Subscribe(_ => ContinueGame())
                .AddTo(this);

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
            if (score <= 0) return;
            if (score % _scoreLineToGetHp == 0)
            {
                //playerのHpを1つ増やす
                _playerPresenter.AddHp(1);
                _scoreLineToGetHp *= nextMagnification;
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
            _gameOverView.gameObject?.SetActive(false);
            //todo フェードを出現させる
            _directionModel.SetIsGameOver(false);
            _directionModel.SetIsGameStart(false);
            _playerPresenter.ResetData();
            _gameStartView.Initialize();
        }

        /// <summary>
        /// ゲームオーバー
        /// </summary>
        void GameOver()
        {
            _gameOverView.gameObject?.SetActive(true);
            _playerPresenter.ChangeDead();
        }

        //クリア処理
    }
}
