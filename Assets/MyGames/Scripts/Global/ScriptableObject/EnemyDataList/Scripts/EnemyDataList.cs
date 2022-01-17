using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDataList", menuName = "ScriptableObject/Create EnemyDataList")]
public class EnemyDataList : ScriptableObject
{
    [SerializeField]
    List<EnemyData> _enemyDataList = new List<EnemyData>();

    public List<EnemyData> GetEnemyDataList => _enemyDataList;
}

[System.Serializable]
public class EnemyData
{
    [SerializeField]
    EnemyType _enemyType;

    [SerializeField]
    int _hp;

    [SerializeField]
    int _power;

    [SerializeField]
    int _speed;

    [SerializeField]
    int _score;

    public EnemyType EnemyType => _enemyType;
    public int Hp => _hp;
    public int Power => _power;
    public int Speed => _speed;
    public int Score => _score;
}