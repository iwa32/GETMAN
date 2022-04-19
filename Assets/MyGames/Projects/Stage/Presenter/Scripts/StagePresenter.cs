using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Zenject;
using GameModel;
using EP = EnemyPresenter;
using SV = StageView;
using BehaviourFactory;
using StageObject;
using SoundManager;
using Cysharp.Threading.Tasks;
using System.Threading;
using static StageData;
using StageDirectingCamera;

namespace StagePresenter
{
    public class StagePresenter : MonoBehaviour
    {
        #region//インスペクターから設定
        [SerializeField]
        [Header("StageDataのScritableObjectを設定")]
        StageDataList _stageDataList;

        [SerializeField]
        [Header("ボス演出用カメラを設定")]
        BossDirectingCamera _bossDirectingCamera;
        #endregion

        #region//フィールド
        StageData _currentStageData;
        SV.StageView _currentStageView;//現在のステージオブジェクトを保持しておく
        EnemyFactory _enemyFactory;//エネミー生成用スクリプト
        PointItemFactory _pointItemFactory;//ポイントアイテム生成用スクリプト
        int _stagePointItemCount;//ステージのポイントアイテムの数をカウント
        Dictionary<EnemyType, int> _countStageEnemies
            = new Dictionary<EnemyType, int>();//ステージのエネミー数
        CancellationTokenSource _cts = new CancellationTokenSource();
        //音声
        ISoundManager _soundManager;
        //モデル
        IStageNumModel _stageNumModel;
        IDirectionModel _directionModel;
        IPointModel _pointModel;
        #endregion

        #region//プロパティ
        public int StageLimitCountTime => _currentStageData.StageLimitCountTime;
        #endregion


        [Inject]
        public void Construct(
            IStageNumModel stageNum,
            IDirectionModel direction,
            IPointModel point,
            ISoundManager soundManager
        )
        {
            _stageNumModel = stageNum;
            _directionModel = direction;
            _pointModel = point;
            _soundManager = soundManager;
        }

        /// <summary>
        /// インスタンス直後の処理
        /// </summary>
        public void ManualAwake()
        {
            _enemyFactory = GetComponent<EnemyFactory>();
            _pointItemFactory = GetComponent<PointItemFactory>();
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        public async UniTask InitializeAsync()
        {
            await SetUpStage();
            //await PlaceBossEnemyToStage();
            Bind();
        }

        /// <summary>
        /// 各Subjectを監視します
        /// </summary>
        void Bind()
        {
            //エネミー、ポイントアイテム、BGMを監視
            BindEnemy();
            BindPointItem();
            BindBgm();

            //獲得ポイント数でゲームクリアを観察
            _pointModel.Point
                .Where(point => point >= _currentStageData.ClearPointCount)
                .Subscribe(_ => _directionModel.SetIsGameClear(true))
                .AddTo(this);
        }

        void BindEnemy()
        {
            //---エネミーの生成---
            foreach (EnemyOption enemyOption in _currentStageData.EnemyOptions)
            {
                PlaceEnemyToStage(enemyOption);//ゲーム開始前に一体場に出しておく

                //一定間隔で自動生成
                IConnectableObservable<long> enemyAppearanceInterval
                    = CreateAppearanceInterval(enemyOption.EnemyAppearanceInterval);
                //生成
                IDisposable enemyAppearanceDisposable
                    = enemyAppearanceInterval
                    .Subscribe(_ => PlaceEnemyToStage(enemyOption))
                    .AddTo(this);

                //ゲーム開始時で生成処理を開始
                _directionModel.IsGameStart
                    .Where(isGameStart => isGameStart == true)
                    .Subscribe(_ => enemyAppearanceInterval.Connect())
                    .AddTo(this);

                //ゲーム終了でエネミーの生成を停止
                this.UpdateAsObservable()
                    .First(_ => _directionModel.IsEndedGame())
                    .Subscribe(_ =>
                        enemyAppearanceDisposable.Dispose())
                    .AddTo(this);
            }
        }

        void BindPointItem()
        {
            //---ポイントアイテムの生成---
            if (_currentStageData.PointGenerationType != PointGenerationType.NO_GENERATION)
            {
                //一定間隔で自動生成
                IConnectableObservable<long> pointItemAppearanceInterval
                    = CreateAppearanceInterval(_currentStageData.PointItemAppearanceInterval);
                //生成
                IDisposable pointItemAppearanceDisposable
                    = pointItemAppearanceInterval
                    .Subscribe(_ => PlacePointItemToStage())
                    .AddTo(this);

                //ゲーム開始時に生成開始
                _directionModel.IsGameStart
                    .Where(isGameStart => isGameStart == true)
                    .Subscribe(_ => pointItemAppearanceInterval.Connect())
                    .AddTo(this);

                //ゲーム終了で生成終了
                this.UpdateAsObservable()
                    .First(_ => _directionModel.IsEndedGame())
                    .Subscribe(_ => pointItemAppearanceDisposable.Dispose())
                    .AddTo(this);
            }
        }

        void BindBgm()
        {
            //---BGM---
            //ゲーム開始時の音声の再生
            _directionModel.IsGameStart
                .Where(isGameStart => isGameStart == true)
                .Subscribe(_ => PlayCurrentStageBgm())
                .AddTo(this);

            //ゲーム終了で音声の停止
            this.UpdateAsObservable()
                .First(_ => _directionModel.IsEndedGame())
                .Subscribe(_ => _soundManager.StopBgm())
                .AddTo(this);
        }

        /// <summary>
        /// 一定間隔で処理を行うためのObservableを作成します
        /// </summary>
        /// <returns></returns>
        IConnectableObservable<long> CreateAppearanceInterval(float interval)
        {
            return Observable
                    .Interval(TimeSpan.FromSeconds(interval))
                    .Publish();
        }

        /// <summary>
        /// 現在のステージのBGMを再生します
        /// </summary>
        void PlayCurrentStageBgm()
        {
            if (_currentStageData == null) return;
            _soundManager.PlayBgm(_currentStageData.BgmType);
        }

        /// <summary>
        /// ステージを設定する
        /// </summary>
        /// <returns></returns>
        async UniTask SetUpStage()
        {
            CancellationToken token = _cts.Token;
            await SetStageData(token);
            await CreateStage(token);
            await SetStageObject(token);
        }

        /// <summary>
        /// ボス敵を設定します
        /// </summary>
        /// <returns></returns>
        public async UniTask PlaceBossEnemyToStage()
        {
            CancellationToken token = _cts.Token;

            if (_currentStageData.BossEnemyPrefab != null)
            {
                _enemyFactory.SetEnemyPool(_currentStageData.BossEnemyPrefab);
                EP.EnemyPresenter enemy = _enemyFactory.CreateTheBoss(_currentStageData.BossEnemyPrefab);
                enemy.SetStageInformation(_currentStageView);

                //演出
                if (_bossDirectingCamera != null)
                    await _bossDirectingCamera.Direct(enemy.transform);
            }
            
            await UniTask.Yield(token);
        }

        /// <summary>
        /// ステージ情報を取得
        /// </summary>
        /// <returns></returns>
        async UniTask SetStageData(CancellationToken token)
        {
            int stageNum = _stageNumModel.StageNum.Value;
            _currentStageData = _stageDataList.GetStageById(stageNum);

            if (_currentStageData == null) _cts.Cancel();

            await UniTask.WaitUntil(() => _currentStageData != null, cancellationToken: token);
        }

        /// <summary>
        /// ステージを生成する
        /// </summary>
        async UniTask CreateStage(CancellationToken token)
        {
            //ステージprefabを取得
            SV.StageView stagePrefab = _currentStageData?.StagePrefab;
            if (stagePrefab == null) _cts.Cancel();
            //生成
            _currentStageView = Instantiate(stagePrefab, Vector3.zero, Quaternion.identity);

            if (_currentStageView == null) _cts.Cancel();

            await UniTask.WaitUntil(() => _currentStageView != null, cancellationToken: token);
        }

        /// <summary>
        /// エネミー、ポイントアイテム、出現地点の設定
        /// </summary>
        /// <returns></returns>
        async UniTask SetStageObject(CancellationToken token)
        {
            //エネミーの種類分設定する
            foreach (EnemyOption enemyOption in _currentStageData.EnemyOptions)
            {
                if (enemyOption.AppearingEnemyPrefab == null)
                {
                    Debug.Log("エネミーを設定してください");
                    _cts.Cancel();
                }

                _enemyFactory.SetEnemyPool(enemyOption.AppearingEnemyPrefab, enemyOption.MaxEnemyCount);
                //出現数を記録
                _countStageEnemies.Add(enemyOption.AppearingEnemyPrefab.Type, 0);
            }

            _pointItemFactory.SetPointItem(_currentStageData.ClearPointCount);
            _currentStageView.InitializeStagePoints();

            await UniTask.Yield(cancellationToken: token);
        }

        /// <summary>
        /// プレイヤーをステージに配置します
        /// </summary>
        public async UniTask PlacePlayerToStage(Transform playerTransform)
        {
            playerTransform.position = _currentStageView.PlayerStartingPoint.position;
            await UniTask.Yield();
        }

        /// <summary>
        /// エネミーをステージに配置します
        /// </summary>
        void PlaceEnemyToStage(EnemyOption enemyOption)
        {
            //エネミーの最大出現数を超えたら生成しない
            var stageEnemyCount = _countStageEnemies[enemyOption.AppearingEnemyPrefab.Type];
            if (stageEnemyCount >= enemyOption.MaxEnemyCount) return;

            //生成
            EP.EnemyPresenter stageEnemy = _enemyFactory?.Create(enemyOption.AppearingEnemyPrefab);

            if (stageEnemy == null) return;

            _countStageEnemies[enemyOption.AppearingEnemyPrefab.Type]++;
            
            //死亡を監視する
            ObserveStageEnemy(stageEnemy);
            //ステージ情報を設定
            stageEnemy.SetStageInformation(_currentStageView);
            stageEnemy.DefaultState();
        }

        /// <summary>
        /// ポイントアイテムを配置します
        /// </summary>
        void PlacePointItemToStage()
        {
            //クリアの必要ポイント数以上は生成しない
            if (_stagePointItemCount >= _currentStageData.ClearPointCount) return;

            PointItem pointItem = _pointItemFactory.Create();

            if (pointItem == null) return;
            _stagePointItemCount++;

            pointItem.transform.position = _currentStageView.GetPointItemAppearancePoint().position;
        }

        /// <summary>
        /// ステージのエネミーを観察します
        /// </summary>
        /// <param name="enemy"></param>
        void ObserveStageEnemy(EP.EnemyPresenter stageEnemy)
        {
            //死亡の監視
            stageEnemy.IsDead
                .Where(isDead => isDead == true)
                .Subscribe(_ => {
                    _countStageEnemies[stageEnemy.Type]--;
                })
                .AddTo(stageEnemy.gameObject);
        }

        /// <summary>
        /// ステージが存在するか確認します
        /// </summary>
        public bool CheckStage(int stageNum)
        {
            return (_stageDataList.GetStageById(stageNum) != null);
        }

        /// <summary>
        /// 次のステージ番号を取得します
        /// </summary>
        /// <returns></returns>
        public int GetNextStageNum()
        {
            return _stageNumModel.StageNum.Value + 1;
        }

        /// <summary>
        /// 次のステージ番号を設定します
        /// </summary>
        public void SetNextStageNum()
        {
            int nextStageNum = GetNextStageNum();
            if (CheckStage(nextStageNum))
            {
                _stageNumModel.SetStageNum(nextStageNum);
            }
        }
    }
}
