using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFactory : MonoBehaviour, IFieldObjGenerateable
{
    #region//インスペクターで設定
    [SerializeField]
    [Header("生成するエネミーのプレハブ")]
    private GameObject enemyObj;

    [SerializeField]
    [Header("出現地点")]
    private GameObject[] enemyBirthPoints;

    [Header("出現頻度")]
    public float birthSpan = 6.0f;

    [Header("エネミーは時間ごとに出現数が増えていく")]
    public bool isIncrease = true;

    [Header("出現SE")]
    public AudioClip birthSE;
    #endregion

    private int phaseCount = 1;//出現した回数

    // Start is called before the first frame update
    void Start()
    {
        if(GameManager.instance == null)
        {
            Debug.Log("GameManagerを設定してください");
        }

        if (enemyObj == null) return;

        //エネミー生成の実行
        StartCoroutine("GenerateObjPerInterval");
    }

    /// <summary>
    /// 決められた間隔ごとにエネミーを生成する
    /// </summary>
    /// <returns></returns>
    IEnumerator GenerateObjPerInterval()
    {
        while(!GameManager.instance.isGameOver)
        {
            //エネミーの生成
            GenerateObj(enemyObj);

            if (isIncrease)
            {
                //コルーチンが廻るたびに生成されるエネミーは増加する
                phaseCount++;
            }

            yield return new WaitForSeconds(birthSpan);
            //出現SE
            GameManager.instance.PlaySE(birthSE);
        }
    }

    /// <summary>
    /// プレハブの生成
    /// </summary>
    /// <param name="obj"></param>
    public void GenerateObj(GameObject obj)
    {
        //n回分、ランダムな出現地点へ生成する
        for (int i = 0; i < phaseCount; i++)
        {
            int random = Random.Range(0, enemyBirthPoints.Length);
            Instantiate(
                obj,
                enemyBirthPoints[random].transform.position,
                Quaternion.identity
            );
        }
    }
}