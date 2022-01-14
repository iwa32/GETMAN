using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

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
    IntReactiveProperty _hp = new IntReactiveProperty();

    [SerializeField]
    IntReactiveProperty _power = new IntReactiveProperty();

    [SerializeField]
    IntReactiveProperty _speed = new IntReactiveProperty();

    [SerializeField]
    IntReactiveProperty _score = new IntReactiveProperty();

    public EnemyType EnemyType => _enemyType;
    public IReactiveProperty<int> Hp => _hp;
    public IReactiveProperty<int> Power => _power;
    public IReactiveProperty<int> Speed => _speed;
    public IReactiveProperty<int> Score => _score;
}