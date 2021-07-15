using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointItem : MonoBehaviour
{
    [SerializeField]
    private PlayerTriggerCheck trigger;

    // Start is called before the first frame update
    void Start()
    {
        if(trigger == null)
        {
            Debug.Log("プレイヤー接触判定がセットされていません");
        }
    }

    // Update is called once per frame
    void Update()
    {
        //毎フレーム回転させる

        if(trigger.isOn)
        {
            if(GameManager.instance != null)
            {
            }
            //ポイントを付与
            //SEを鳴らす
            //消滅
        }
    }
}
