using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Zenject;
using PlayerModel;

namespace PlayerPresenter
{
    public class PlayerPresenter : MonoBehaviour
    {
        [SerializeField]
        [Header("武器アイコンのUIを設定")]
        WeaponView _weaponView;

        [SerializeField]
        [Header("HPのUIを設定")]
        HpView _hpView;

        [SerializeField]
        [Header("スコアのUIを設定")]
        ScoreView _scoreView;

        [SerializeField]
        [Header("獲得ポイントのUIを設定")]
        PointView _pointView;

        [SerializeField]
        [Header("プレイヤーの状態スクリプトを設定")]
        StateView _stateView;

        IWeaponModel _weaponModel;
        IHpModel _hpModel;
        IScoreModel _scoreModel;
        IPointModel _pointModel;
        IStateModel _stateModel;

        [Inject]
        public void Construct(
            IWeaponModel weapon,
            IHpModel hp,
            IScoreModel score,
            IPointModel point,
            IStateModel state
        )
        {
            _weaponModel = weapon;
            _hpModel = hp;
            _scoreModel = score;
            _pointModel = point;
            _stateModel = state;
        }

        //modelとuiの紐付け
        void Awake()
        {
            _hpModel.Hp.Subscribe(hp => _hpView.SetHpGauge(hp));
            _scoreModel.Score.Subscribe(score => _scoreView.SetScore(score));
            _pointModel.Point.Subscribe(point => _pointView.SetPointGauge(point));
        }

        //todo 後で上位クラスから初期化処理を呼ぶように変更する
        void Start()
        {
            //Initialize();
        }

        //void Initialize()
        //{
        //    _hpView.SetHeartGauge(_hpModel.Hp.Value);
        //}
    }
}