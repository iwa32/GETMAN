using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    #region//インスペクターから設定
    public float speed = 1.0f;
    #endregion

    #region//コンポーネント
    private InputAction _moveAction, _lookAction, _fireAction;
    private Animator anim;
    private PlayerInput pInput;
    private Rigidbody rb;
    #endregion

    #region//タグ
    private string enemyTag = "Enemy";
    #endregion

    #region//フラグ
    private bool isRun;
    private bool isDown;
    private bool isContinue;
    #endregion

    #region//タイマー
    private float continueTime;
    private float blinkTime;//点滅時間
    #endregion

    #region//Vector
    private Vector3 inputDirection;
    private Vector3 playerPos;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        playerPos = transform.position;
        //Input System
        pInput = GetComponent<PlayerInput>();
        InputActionMap actionMap = pInput.currentActionMap;
        //アクションを取得
        _moveAction = actionMap["Move"];
        //_lookAction = actionMap["Look"];
        //_fireAction = actionMap["Fire"];
    }

    private void Update()
    {
        //コントローラーの値を取得
        Vector2 moveVector = _moveAction.ReadValue<Vector2>();
        //入力情報を3次元として取得
        inputDirection = new Vector3(moveVector.x, 0, moveVector.y) * speed;
        //Vector2 look = _lookAction.ReadValue<Vector2>();
        //bool fire = _fireAction.triggered;

        PlayerBlinks();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (GameManager.instance.isGameOver) return;

        RotatePlayer();
        MovePlayer();
        SetAnimation();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag(enemyTag))
        {
            PlayerDown();
        }
    }

    /// <summary>
    /// プレイヤーの点滅
    /// </summary>
    private void PlayerBlinks()
    {
        if (!isContinue) return;

        //明滅ついている時に戻る③
        if(blinkTime > 0.2f)
        {
            SetActiveToAllChild(true);
            blinkTime = 0.0f;
        }
        else if(blinkTime > 0.1f)
        {
            //消えている時②
            SetActiveToAllChild(false);
        }
        else
        {
            //明滅ついている時①
            SetActiveToAllChild(true);
        }

        //1秒経ったら明滅終わり
        if(continueTime > 1.0f)
        {
            isContinue = false;
            continueTime = 0.0f;
            blinkTime = 0.0f;
            SetActiveToAllChild(true);
        }
        else
        {
            blinkTime += Time.deltaTime;
            continueTime += Time.deltaTime;
        }
    }

    /// <summary>
    /// 子要素を全てアクティブ・非アクティブにする
    /// </summary>
    /// <param name="isActive"></param>
    private void SetActiveToAllChild(bool isActive)
    {
        foreach(Transform child in gameObject.transform)
        {
            child.gameObject.SetActive(isActive);
        }
    }

    /// <summary>
    /// プレイヤーがダウンする
    /// </summary>
    private void PlayerDown()
    {
        //すでにダウン中ならダウンしない
        if (isDown) return;

        isDown = true;
        if (GameManager.instance == null) return;
        //HPを減らす
        GameManager.instance.ReduceHeartNum();
    }


    /// <summary>
    /// コンティニュー待機中か
    /// </summary>
    /// <returns></returns>
    public bool IsContinueWaiting()
    {
        return IsDownAnimEnd();
    }

    /// <summary>
    /// ダウンアニメーションが終わっているか
    /// </summary>
    /// <returns>anim終了フラグ</returns>
    private bool IsDownAnimEnd()
    {
        if(isDown && anim != null)
        {
            AnimatorStateInfo currentInfo = anim.GetCurrentAnimatorStateInfo(0);
            if(currentInfo.IsName("Down"))
            {
                //アニメーションが終了したら
                if(currentInfo.normalizedTime >= 1)
                {
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// プレイヤーをコンティニュー
    /// </summary>
    public void ContinuePlayer()
    {
        isDown = false;
        isRun = false;
        isContinue = true;
        anim.Play("Idle");
        //正面を向く
        transform.LookAt(new Vector3(0, 1, 0));
    }

    /// <summary>
    /// プレイヤーの回転
    /// </summary>
    void RotatePlayer()
    {
        //プレイヤーの進行方向の座標の差分を取得
        Vector3 diffPos = transform.position - playerPos;
        //入力があったら回転量を取得し、プレイヤーを回転させる
        if (inputDirection.x != 0 || inputDirection.z != 0)
        {
            if (diffPos.magnitude <= 0.01f) return;
            rb.rotation = Quaternion.LookRotation(diffPos);
        }
        //プレイヤーの位置を更新
        playerPos = transform.position;
    }

    /// <summary>
    /// プレイヤーの移動
    /// </summary>
    void MovePlayer()
    {
        if (isDown) return;

        //入力があった場合
        if (inputDirection != Vector3.zero)
        {
            isRun = true;
            rb.velocity = inputDirection;
        }
        else
        {
            //idle状態
            isRun = false;
        }
    }

    void SetAnimation()
    {
        anim.SetBool("IsRun", isRun);
        anim.SetBool("IsDown", isDown);
    }
}
