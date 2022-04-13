using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemyDataList
{
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
        float _speed;

        [SerializeField]
        int _score;

        [SerializeField]
        [Tooltip("アイテムドロップ率")]
        float _itemDropRate;

        [SerializeField]
        [Tooltip("ドロップアイテムのプレハブを設定")]
        GameObject _dropItem;


        public EnemyType EnemyType => _enemyType;
        public int Hp => _hp;
        public int Power => _power;
        public float Speed => _speed;
        public int Score => _score;
        public float ItemDropRate => _itemDropRate;
        public GameObject DropItem => _dropItem;
    }
}
