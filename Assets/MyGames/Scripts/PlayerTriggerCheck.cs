using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTriggerCheck : MonoBehaviour
{
    [HideInInspector]
    public bool isOn;//プレイヤーが接触しているかのフラグ

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            isOn = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isOn = false;
        }
    }
}
