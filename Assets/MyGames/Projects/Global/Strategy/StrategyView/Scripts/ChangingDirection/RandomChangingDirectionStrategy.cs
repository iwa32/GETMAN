using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StrategyView
{
    /// <summary>
    /// 進行方向をランダムに変えるクラス
    /// </summary>
    public class RandomChangingDirectionStrategy : ChangingDirectionStrategy
    {
        [SerializeField]
        [Header("何通りの方向に向きを変えるか")]
        int _directionRange = 4;

        [SerializeField]
        [Header("方向変換する角度を設定")]
        int _directionAngle = 90;

        public override void Strategy()
        {
            //ランダムな方向に向きを変えます※値は1から開始
            int direction = Random.Range(1, _directionRange + 1);
            int dirAngle = _directionAngle;

            dirAngle *= direction;

            //すでに同じ方向を向いてたら処理を行わない
            if (transform.localEulerAngles.x == dirAngle) return;
            //オイラー値をQuaternionに変換する。引数はz, x, y
            transform.rotation = Quaternion.Euler(0, dirAngle, 0);
        }
    }
}
