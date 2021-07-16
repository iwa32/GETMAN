using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    #region//インスペクターから設定
    [Header("移動速度")]
    public float speed;
    [SerializeField]
    private EnemyCollisionCheck checkCollision;
    #endregion

    private Rigidbody rb;
    private Animator anim;

    #region//アニメーションフラグ
    private bool isWalk;
    private bool isRun;
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        WalkToForward();
        //進行方向を変える
        if(checkCollision.isOn)
        {
            //向きを回転
            RandomRotateEnemy();
        }

        SetAnimation();
    }


    /// <summary>
    /// 前方に歩く
    /// </summary>
    void WalkToForward()
    {
        //velocityによる移動
        rb.velocity = transform.forward * speed;
        isWalk = true;
    }

    /// <summary>
    /// アニメーションの設定
    /// </summary>
    void SetAnimation()
    {
        anim.SetBool("IsWalk", isWalk);
    }

    /// <summary>
    /// 進行方向を変える
    /// </summary>
    void RandomRotateEnemy()
    {
        //進行方向はランダム
        int dice = RandomDice(1, 5);
        int dirAngle = 90;

        dirAngle *= dice;

        //すでに同じ方向を向いてたら処理を行わない
        if (transform.localEulerAngles.x == dirAngle) return;
        //オイラー値をQuaternionに変換する。引数はz, x, y
        transform.rotation = Quaternion.Euler(0, dirAngle, 0);
    }

    /// <summary>
    /// ランダムな数値を算出
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns>ランダムな数値を出力</returns>
    int RandomDice(int min, int max)
    {
        return Random.Range(min, max);
    }
}
