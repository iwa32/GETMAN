using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour,
    IPlayerWeapon
{
    [SerializeField]
    [Header("武器の攻撃力を設定")]
    int _power;

    public int Power => _power;

    /// <summary>
	/// パワーの設定
	/// </summary>
	/// <param name="power"></param>
    public void SetPower(int power)
    {
        _power = power;
    }
}
