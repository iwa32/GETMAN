using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    #region//インスペクターから設定
    public float speed = 100.0f;
    #endregion

    #region//コンポーネント
    private InputAction _moveAction, _lookAction, _fireAction;
    private Animator anim;
    private PlayerInput pInput;
    private Rigidbody rb;
    #endregion

    #region//フラグ
    private bool isRun;
    #endregion
    private Vector3 inputDirection;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        //Input System
        pInput = GetComponent<PlayerInput>();
        InputActionMap actionMap = pInput.currentActionMap;
        //アクションを取得
        _moveAction = actionMap["Move"];
        _lookAction = actionMap["Look"];
        _fireAction = actionMap["Fire"];
    }

    private void Update()
    {
        //コントローラーの値を取得
        Vector2 moveVector = _moveAction.ReadValue<Vector2>();
        //Vector2 look = _lookAction.ReadValue<Vector2>();
        //bool fire = _fireAction.triggered;

        inputDirection = new Vector3(moveVector.x, 0, moveVector.y) * speed;
        // inputDirection.z = Input.GetAxis("Horizontal");
        // inputDirection.x = Input.GetAxis("Vertical");

        SetAnimation();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //入力があった場合
        if(inputDirection != Vector3.zero)
        {
            isRun = true;
            rb.AddForce(inputDirection, ForceMode.Acceleration);
        }
        else
        {
            //idle状態
            isRun = false;
        }

        //Debug.Log(string.Format("moveVector:{0} + Look:{1} + Fire:{2}", moveVector, look, fire));
    }

    void SetAnimation()
    {
        anim.SetBool("IsRun", isRun);
    }
}
