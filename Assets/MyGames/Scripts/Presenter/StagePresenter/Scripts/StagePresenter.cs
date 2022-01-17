using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Zenject;
using GameModel;
using StageView;

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
        IStageNumModel _stageNumModel;
        StageView.StageView _currentStageView;//現在のステージを保持しておく
        BoolReactiveProperty _isCreatedStage = new BoolReactiveProperty();
        BoolReactiveProperty _isPlacedPlayer = new BoolReactiveProperty();
        #endregion

        #region//プロパティ
        public IReadOnlyReactiveProperty<bool> IsCreatedState => _isCreatedStage;
        public IReadOnlyReactiveProperty<bool> IsPlacedPlayer => _isPlacedPlayer;
        #endregion


        [Inject]
        public void Construct(
            IStageNumModel stageNum
        )
        {
            _stageNumModel = stageNum;
        }

        /// <summary>
        /// インスタンス直後の処理
        /// </summary>
        public void ManualAwake()
        {
            Debug.Log("awake");
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        public void Initialize()
        {
            CreateStage();
            Bind();
        }

        void Bind()
        {
            Debug.Log("bind");
        }

        /// <summary>
        /// ステージを生成する
        /// </summary>
        void CreateStage()
        {
            int stageNum = _stageNumModel.StageNum.Value;

            //ステージデータを取得
            StageView.StageView stagePrefab = _stageDataList.GetStageById(stageNum)?.StagePrefab;
            if (stagePrefab == null) return;

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
    }
}
