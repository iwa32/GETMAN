using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalInterface;
using EnemyStates;
using EnemyActions;

namespace EnemyPresenter
{
    public class TurtleShellPresenter : EnemyPresenter, IPlayerAttacker
    {
        #region//インスペクターから設定
        #endregion

        #region//フィールド
        TurtleShellStates _turtleShellStates;
        TurtleShellActions _turtleShellActions;
        #endregion

        #region//プロパティ
        public int Power => _powerModel.Power.Value;
        #endregion


        // Start is called before the first frame update
        void Awake()
        {
            base.ManualAwake();

            _turtleShellStates = GetComponent<TurtleShellStates>();
            _turtleShellActions = GetComponent<TurtleShellActions>();
            _turtleShellStates.ManualAwake();
            _turtleShellActions.ManualAwake();
        }

        void Start()
        {
            Initialize();
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        public void Initialize()
        {
            _turtleShellStates.Initialize();
        }

        #region //overrideMethod

        public override void CheckCollider(Collider collider)
        {
            //武器に接触でダメージを受ける
            CheckPlayerWeaponBy(collider);
        }

        public override void CheckCollision(UnityEngine.Collision collision)
        {

        }
        #endregion
    }
}
