using System;
using UnityEngine;

public interface IFieldObjGenerateable
{
    /// <summary>
	/// プレハブの生成
	/// </summary>
	/// <param name="obj"></param>
    void GenerateObj(GameObject obj);
}
