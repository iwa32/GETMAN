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
    #endregion

    #region//その他
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
    }

    // Update is called once per frame
    void FixedUpdate()
    {
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
    /// プレイヤーがダウンする
    /// </summary>
    private void PlayerDown()
    {
        isDown = true;
        if (GameManager.instance == null) return;
        //HPを減らす
        GameManager.instance.ReduceHeartNum();
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
