using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerView
{
    public class HpView : MonoBehaviour
    {
        [SerializeField]
        [Header("HPのUIプレハブを設定")]
        private GameObject hpPrefab;//hpUIのプレハブ

        [SerializeField]
        [Header("HPUIの格納場所を設定")]
        Transform hpTransform;

        /// <summary>
        /// HPの設定を行います
        /// </summary>
        /// <param name="hp"></param>
        public void SetHpGauge(int hp)
        {
            //体力を一旦削除
            for (int i = 0; i < hpTransform.childCount; i++)
            {
                Destroy(hpTransform.GetChild(i).gameObject);
            }
            //現在の体力数分のゲージを作成
            for (int i = 0; i < hp; i++)
            {
                Instantiate(hpPrefab, hpTransform);//prefab, 配置場所
            }
        }

        //ダメージ分だけ削除
        public void ReduceHp(int damage)
        {
            for (int i = 0; i < damage; i++)
            {
                Destroy(hpTransform.GetChild(i).gameObject);
            }
        }
    }
}
