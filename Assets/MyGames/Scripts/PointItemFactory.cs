using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointItemFactory : MonoBehaviour, IFieldObjGenerateable
{
    #region//インスペクターで設定
    [SerializeField]
    [Header("生成するポイントのプレハブ")]
    private GameObject pointItemObj;

    [SerializeField]
    [Header("出現地点")]
    private GameObject[] pointItemBirthPoints;

    [Header("出現頻度")]
    public float birthSpan = 12.0f;

    #endregion

    private int phaseCount = 1;//出現した回数

    // Start is called before the first frame update
    void Start()
    {
        if (GameManager.instance == null)
        {
            Debug.Log("GameManagerを設定してください");
        }

        if (pointItemObj == null) return;

        //ポイントアイテム生成の実行
        StartCoroutine("GenerateObjPerInterval");
    }

    /// <summary>
    /// 決められた間隔ごとにポイントアイテムを生成する
    /// </summary>
    /// <returns></returns>
    IEnumerator GenerateObjPerInterval()
    {
        while (!GameManager.instance.isGameOver)
        {
            //ポイントアイテムの生成
            GenerateObj(pointItemObj);

            yield return new WaitForSeconds(birthSpan);
        }
    }

    /// <summary>
    /// プレハブの生成
    /// </summary>
    /// <param name="obj"></param>
    public void GenerateObj(GameObject obj)
    {
        //ランダムな出現地点へ生成する
        int random = Random.Range(0, pointItemBirthPoints.Length);
        Instantiate(
            obj,
            pointItemBirthPoints[random].transform.position,
            Quaternion.identity
        );
    }
}
