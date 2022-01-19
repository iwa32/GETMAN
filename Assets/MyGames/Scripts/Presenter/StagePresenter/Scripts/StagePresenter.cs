using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Zenject;
using GameModel;
using EP = EnemyPresenter;
using SV = StageView;
using EF = EnemyFactory;

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
        EF.EnemyFactory _enemyFactory;//エネミー生成用スクリプト
        List<EP.EnemyPresenter> _stageEnemyList = new List<EP.EnemyPresenter>();//ステージの敵を格納する
        //フラグ
        BoolReactiveProperty _isCreatedStage = new BoolReactiveProperty();
        BoolReactiveProperty _isPlacedPlayer = new BoolReactiveProperty();
        //モデル
        IStageNumModel _stageNumModel;
        IDirectionModel _directionModel;
        #endregion

        #region//プロパティ
        public IReadOnlyReactiveProperty<bool> IsCreatedState => _isCreatedStage;
        public IReadOnlyReactiveProperty<bool> IsPlacedPlayer => _isPlacedPlayer;
        #endregion


        [Inject]
        public void Construct(
            IStageNumModel stageNum,
            IDirectionModel direction
        )
        {
            _stageNumModel = stageNum;
            _directionModel = direction;
        }

        /// <summary>
        /// インスタンス直後の処理
        /// </summary>
        public void ManualAwake()
        {
            _enemyFactory = GetComponent<EF.EnemyFactory>();
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
            //エネミーを一定間隔で生成する準備
            IConnectableObservable<long> EnemyAppearanceInterval
                = CreateAppearanceInterval(_currentStageData.EnemyAppearanceInterval);

            //ゲーム開始でエネミーの生成開始
            _directionModel.IsGameStart
                .Where(isGameStart => isGameStart == true)
                .Subscribe(_ => EnemyAppearanceInterval.Connect())
                .AddTo(this);

            IDisposable enemyAppearanceDisposable =
                EnemyAppearanceInterval
                .Subscribe(_ => PlaceEnemyToStage())
                .AddTo(this);

            //ゲームオーバーで生成を停止します
            _directionModel.IsGameOver
                .Where(isGameOver => isGameOver == true)
                .Subscribe(_ => enemyAppearanceDisposable.Dispose())
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
            _currentStageView?.SetPlayerToStartPoint(playerTransform);
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
            _currentStageView?.SetEnemyToRandomAppearancePoint(stageEnemy.transform);
        }

        /// <summary>
        /// ステージのエネミーを観察します
        /// </summary>
        /// <param name="enemy"></param>
        void ObserveStageEnemy(EP.EnemyPresenter stageEnemy)
        {
            //死亡の確認
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
    }
}
