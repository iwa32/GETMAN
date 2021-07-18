using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollisionCheck : MonoBehaviour
{
    [HideInInspector]
    public bool isOn;//接触判定
    #region//タグ
    private string enemyTag = "Enemy";
    private string wallTag = "Wall";
    private string playerWaitLocationTag = "PlayerWaitLocation";
    #endregion


    private void OnTriggerEnter(Collider other)
    {
        if (HasTargetCompareTag(other))
        {
            isOn = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (HasTargetCompareTag(other))
        {
            isOn = false;
        }
    }

    /// <summary>
    /// 対象との接触を検知する
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    private bool HasTargetCompareTag(Collider other)
    {
        return other.CompareTag(enemyTag)
            || other.CompareTag(wallTag)
            || other.CompareTag(playerWaitLocationTag);
    }
}
