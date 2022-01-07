using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Zenject;
using PlayerModel;
using PlayerView;
using static PlayerState;

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

        [SerializeField]
        [Header("プレイヤーの入力取得スクリプトを設定")]
        InputView _inputView;

        Rigidbody _rigidBody;
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

        void Awake()
        {
            _rigidBody = GetComponent<Rigidbody>();
        }

        //todo 後で上位クラスから初期化処理を呼ぶように変更する
        void Start()
        {
            Initialize();
        }

        void Initialize()
        {
            Bind();
        }

        /// <summary>
        /// 監視し、ModelとUIの紐付けを行う
        /// </summary>
        void Bind()
        {
            //modelの監視
            _hpModel.Hp.Subscribe(hp => _hpView.SetHpGauge(hp));
            _scoreModel.Score.Subscribe(score => _scoreView.SetScore(score));
            _pointModel.Point.Subscribe(point => _pointView.SetPointGauge(point));
            _stateModel.State.Subscribe(state => RegisterStateAction(state));

            //viewの監視
            _inputView.InputDirection.Subscribe(input => ChangeStateByInput(input));
        }

        void FixedUpdate()
        {
            _stateView.Action();
        }

        /// <summary>
        /// 入力の有無でプレイヤーの状態を切り替えます
        /// </summary>
        /// <param name="input"></param>
        void ChangeStateByInput(Vector2 input)
        {
            if (input.magnitude != 0)
                _stateModel.SetState(RUN);
            else
                _stateModel.SetState(WAIT);
        }

        /// <summary>
        /// 状態ごとの処理を登録します
        /// </summary>
        /// <param name="state"></param>
        void RegisterStateAction(PlayerState state)
        {
            _stateView.ChangeState(state);
            _stateView.SetDelAction(GetDelActionByState(state));
        }

        /// <summary>
        /// 状態ごとに必要な処理を取得します
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        Action GetDelActionByState(PlayerState state)
        {
            if (state == RUN) return Run;
            return Wait;
        }

        /// <summary>
        /// 走ります
        /// </summary>
        public void Run()
        {
            Vector2 input = _inputView.InputDirection.Value;
            Move(input);
            Rotation(input);
        }

        /// <summary>
        /// 移動します
        /// </summary>
        /// <param name="input"></param>
        void Move(Vector2 input)
        {
            //入力があった場合
            if (input != Vector2.zero)
            {
                Vector3 movePos = new Vector3(input.x, 0, input.y);
                _rigidBody.velocity = movePos * 10f;
            }
        }

        /// <summary>
        /// 回転します
        /// </summary>
        /// <param name="input"></param>
        void Rotation(Vector2 input)
        {
            _rigidBody.rotation = Quaternion.LookRotation(new Vector3(input.x, 0, input.y));
        }

        /// <summary>
        /// 待機状態
        /// </summary>
        public void Wait()
        {
            Debug.Log("Wait");
        }
    }
}