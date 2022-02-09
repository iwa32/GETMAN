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
        List<EP.EnemyPresenter> _stageEnemyList = new List<EP.EnemyPresenter>();//ステージの敵を格納する
        int _stagePointItemCount;//ステージのポイントアイテムの数をカウント
        //フラグ
        BoolReactiveProperty _isCreatedStage = new BoolReactiveProperty();
        BoolReactiveProperty _isPlacedPlayer = new BoolReactiveProperty();
        //音声
        ISoundManager _soundManager;
        //モデル
        IStageNumModel _stageNumModel;
        IDirectionModel _directionModel;
        IPointModel _pointModel;
        #endregion

        #region//プロパティ
        public IReadOnlyReactiveProperty<bool> IsCreatedState => _isCreatedStage;
        public IReadOnlyReactiveProperty<bool> IsPlacedPlayer => _isPlacedPlayer;
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
        public void Initialize()
        {
            SetCurrentStageData();
            CreateStage();
            Bind();
        }

        void Bind()
        {
            //エネミーを一定間隔で自動生成
            IConnectableObservable<long> enemyAppearanceInterval
                = CreateAppearanceInterval(_currentStageData.EnemyAppearanceInterval);
            //ポイントアイテムを一定間隔で自動生成
            IConnectableObservable<long> pointItemAppearanceInterval
                = CreateAppearanceInterval(_currentStageData.PointItemAppearanceInterval);


            //ゲーム開始時の処理
            _directionModel.IsGameStart
                .Where(isGameStart => isGameStart == true)
                .Subscribe(_ =>
                {
                    //エネミーとポイントアイテムの自動生成開始
                    enemyAppearanceInterval.Connect();
                    pointItemAppearanceInterval.Connect();
                    //音声の再生
                    PlayCurrentStageBgm();
                })
                .AddTo(this);


            IDisposable enemyAppearanceDisposable
                = enemyAppearanceInterval
                .Subscribe(_ => PlaceEnemyToStage())
                .AddTo(this);

            IDisposable pointItemAppearanceDisposable
                = pointItemAppearanceInterval
                .Subscribe(_ => PlacePointItemToStage())
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
                    pointItemAppearanceDisposable.Dispose();
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
        /// 現在のステージ情報を設定します
        /// </summary>
        void SetCurrentStageData()
        {
            int stageNum = _stageNumModel.StageNum.Value;
            //ステージの設定
            _currentStageData = _stageDataList.GetStageById(stageNum);
            //出現エネミーの設定
            RegisterAppearingEnemyForStage();
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
        /// ステージで出現するエネミーを登録します
        /// </summary>
        void RegisterAppearingEnemyForStage()
        {
            if (_currentStageData.AppearingEnemies.Length == 0)
                Debug.Log("エネミーの種類を設定してください");
            _enemyFactory.SetEnemyDataByType(_currentStageData.AppearingEnemies);
        }

        /// <summary>
        /// ステージを生成する
        /// </summary>
        void CreateStage()
        {
            //ステージデータを取得
            SV.StageView stagePrefab = _currentStageData?.StagePrefab;
            if (stagePrefab == null) return;
            //生成
            _currentStageView = Instantiate(stagePrefab, Vector3.zero, Quaternion.identity);
            _isCreatedStage.Value = true;
        }

        /// <summary>
        /// プレイヤーをステージに配置します
        /// </summary>
        public void PlacePlayerToStage(Transform playerTransform)
        {
            playerTransform.position = _currentStageView.PlayerStartingPoint.position;
            _isPlacedPlayer.Value = true;
        }

        /// <summary>
        /// エネミーをステージに配置します
        /// </summary>
        void PlaceEnemyToStage()
        {
            //エネミーの最大出現数を超えたら生成しない
            if (_stageEnemyList.Count >= _currentStageData.MaxEnemyCount) return;

            //生成
            EP.EnemyPresenter stageEnemy = _enemyFactory?.Create();

            //リストに追加し、観察対象にする
            _stageEnemyList.Add(stageEnemy);
            ObserveStageEnemy(stageEnemy);

            //配置
            Transform appearancePoint = _currentStageView?.GetEnemyAppearancePoint();
            stageEnemy?.SetTransform(appearancePoint);
        }

        /// <summary>
        /// ポイントアイテムを配置します
        /// </summary>
        void PlacePointItemToStage()
        {
            //クリアの必要ポイント数以上は生成しない
            if (_stagePointItemCount >= _currentStageData.ClearPointCount) return;

            PointItem pointItem = _pointItemFactory.Create();
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
                    DeleteEnemy(stageEnemy);
                })
                .AddTo(stageEnemy.gameObject);
        }

        /// <summary>
        /// エネミーを削除します
        /// </summary>
        /// <param name="stageEnemy"></param>
        void DeleteEnemy(EP.EnemyPresenter stageEnemy)
        {
            Destroy(stageEnemy.gameObject);
            //リストの参照も削除します
            _stageEnemyList
                .RemoveAll(enemy => (
                enemy.gameObject.GetInstanceID() == stageEnemy.gameObject.GetInstanceID())
                );
        }

        /// <summary>
        /// 次のステージが存在するか確認します
        /// </summary>
        public bool CheckStage(int stageNum)
        {
            return (_stageDataList.GetStageById(stageNum) != null);
        }
    }
}
