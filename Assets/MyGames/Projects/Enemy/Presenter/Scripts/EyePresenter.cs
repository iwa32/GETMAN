using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalInterface;
using EnemyStates;
using EnemyActions;
using EnemyDataList;


namespace EnemyPresenter
{
    public class EyePresenter : EnemyPresenter
    {
        #region//フィールド
        EyeStates _eyeStates;
        EyeActions _eyeActions;
        #endregion

        // Start is called before the first frame update
        void Awake()
        {
            base.ManualAwake();

            _eyeStates = GetComponent<EyeStates>();
            _eyeActions = GetComponent<EyeActions>();

            _eyeStates.ManualAwake();
            _eyeActions.ManualAwake();
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        public override void Initialize(EnemyData data)
        {
            base.Initialize(data);
            _eyeStates.Initialize();
            _eyeActions.Initialize();
        }

        public override void CheckCollider(Collider collider)
        {
            //武器に接触でダメージを受ける
            CheckPlayerWeaponBy(collider);
        }

        public override void CheckCollision(UnityEngine.Collision collision)
        {
            
        }
    }
}
