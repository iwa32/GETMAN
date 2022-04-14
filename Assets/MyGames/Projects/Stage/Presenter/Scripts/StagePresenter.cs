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

namespace StagePresenter
{
    public class StagePresenter : MonoBehaviour
    {
        #region//インスペクターから設定
        [SerializeField]
        [Header("StageDataのScritableObjectを設定")]
        StageDataList _stageDataList;
        #endregion

        #region//フィールド
        StageData _currentStageData;
        SV.StageView _currentStageView;//現在のステージオブジェクトを保持しておく
        EnemyFactory _enemyFactory;//エネミー生成用スクリプト
        PointItemFactory _pointItemFactory;//ポイントアイテム生成用スクリプト
        int _stagePointItemCount;//ステージのポイントアイテムの数をカウント
        int _stageEnemyCount;//ステージのエネミー数
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
            Bind();
        }

        void Bind()
        {
            //---エネミーの生成---
            //一定間隔で自動生成
            IConnectableObservable<long> enemyAppearanceInterval
                = CreateAppearanceInterval(_currentStageData.EnemyAppearanceInterval);
            //生成
            IDisposable enemyAppearanceDisposable
                = enemyAppearanceInterval
                .Subscribe(_ => PlaceEnemyToStage())
                .AddTo(this);


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

            //ゲーム開始時の処理
            _directionModel.IsGameStart
                .Where(isGameStart => isGameStart == true)
                .Subscribe(_ =>
                {
                    //エネミーとポイントアイテムの自動生成開始
                    enemyAppearanceInterval.Connect();
                    //音声の再生
                    PlayCurrentStageBgm();
                })
                .AddTo(this);

            //獲得ポイント数でゲームクリアを観察
            _pointModel.Point
                .Where(point => point >= _currentStageData.ClearPointCount)
                .Subscribe(_ => _directionModel.SetIsGameClear(true))
                .AddTo(this);

            //ゲーム終了で
            this.UpdateAsObservable()
                .First(_ => _directionModel.IsEndedGame())
                .Subscribe(_ =>
                {
                    //エネミーとポイントアイテムの自動生成を停止する
                    enemyAppearanceDisposable.Dispose();
                    //音声の停止
                    _soundManager.StopBgm();
                })
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
        /// エネミー、ポイントアイテムの設定
        /// </summary>
        /// <returns></returns>
        async UniTask SetStageObject(CancellationToken token)
        {
            //出現エネミー、ポイントアイテムの設定
            if (_currentStageData.AppearingEnemyPrefabs.Length == 0)
            {
                Debug.Log("エネミーを設定してください");
                _cts.Cancel();
            }

            _enemyFactory.SetEnemyData(_currentStageData.AppearingEnemyPrefabs, _currentStageData.MaxEnemyCount);
            _pointItemFactory.SetPointItemData(_currentStageData.ClearPointCount);

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
        void PlaceEnemyToStage()
        {
            //エネミーの最大出現数を超えたら生成しない
            if (_stageEnemyCount >= _currentStageData.MaxEnemyCount) return;

            //生成
            EP.EnemyPresenter stageEnemy = _enemyFactory?.Create();

            if (stageEnemy == null) return;
            //死亡を監視する
            _stageEnemyCount++;
            ObserveStageEnemy(stageEnemy);

            //配置
            Transform appearancePoint = _currentStageView?.GetEnemyAppearancePoint();
            stageEnemy?.SetTransform(appearancePoint);
            stageEnemy.SetPatrolPoints(_currentStageView?.PatrollPoints);
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
                    _stageEnemyCount--;
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
