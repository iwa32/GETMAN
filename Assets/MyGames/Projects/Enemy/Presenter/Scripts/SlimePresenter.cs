using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalInterface;
using EnemyStates;
using EnemyActions;
using EnemyDataList;

namespace EnemyPresenter
{
    public class SlimePresenter : EnemyPresenter, IPlayerAttacker
    {
        #region//インスペクターから設定
        #endregion

        #region//フィールド
        SlimeStates _slimeStates;
        SlimeActions _slimeActions;
        #endregion

        #region//プロパティ
        public int Power => _powerModel.Power.Value;
        #endregion


        // Start is called before the first frame update
        void Awake()
        {
            base.ManualAwake();
            _slimeStates = GetComponent<SlimeStates>();
            _slimeActions = GetComponent<SlimeActions>();

            _slimeStates.ManualAwake();
            _slimeActions.ManualAwake();
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        public override void Initialize(EnemyData data)
        {
            base.Initialize(data);
            _slimeStates.Initialize();
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
