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
    #endregion


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(enemyTag) || other.CompareTag(wallTag))
        {
            isOn = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(enemyTag) || other.CompareTag(wallTag))
        {
            isOn = false;
        }
    }
}
