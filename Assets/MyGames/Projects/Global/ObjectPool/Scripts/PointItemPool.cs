using System.Collections;
using System.Collections.Generic;
using ObjectPool;
using StageObject;
using UnityEngine;
using Zenject;

public class PointItemPool : IPointItemPool
{
    public List<PointItem> _pointItemList = new List<PointItem>();

    [Inject]
    DiContainer container;//動的生成したデータにDIできるようにする

    /// <summary>
    /// オブジェクトプールを作成する
    /// </summary>
    /// <param name="pointItemPrefab"></param>
    /// <param name="maxObjectCount"></param>
    public void CreatePool(PointItem pointItemPrefab, int maxPointItemCount)
    {
        //それぞれのenemyを最大出現数分作成しpoolします
        for (int j = 0; j < maxPointItemCount; j++)
        {
            PointItem pointItem
                = Create(pointItemPrefab);

            _pointItemList.Add(pointItem);
            pointItem.gameObject?.SetActive(false);
        }
    }

    /// <summary>
    /// プールを取得します
    /// </summary>
    /// <returns></returns>
    public PointItem GetPool()
    {
        foreach (PointItem pointItem in _pointItemList)
        {
            if (pointItem.gameObject.activeSelf)
            {
                continue;
            }

            pointItem.gameObject?.SetActive(true);

            return pointItem;
        }

        return null;
    }

    /// <summary>
    /// ポイントアイテムの作成
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    PointItem Create(PointItem pointItemPrefab)
    {
        PointItem pointItem
            = container.InstantiatePrefab(
                        pointItemPrefab,
                        Vector3.zero,
                        Quaternion.identity,
                        null
            )
            .GetComponent<PointItem>();

        return pointItem;
    }
}
