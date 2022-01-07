using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour,
    IAttacker
{
    [SerializeField]
    [Header("攻撃力を設定する")]
    int _power = 1;

    public int Power => _power;
    
}
