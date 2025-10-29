using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    MoveGage moveGage;

    Rigidbody2D rb;
    [SerializeField] Transform cameraTf;

    const float MOVE_POWER = 500;

    [SerializeField] float moveSpeed = 1;

    /// <summary>
    /// 初期化
    /// </summary>
    void Init()
    {
        rb = GetComponent<Rigidbody2D>();
        moveGage = GetComponent<MoveGage>();
    }

    private void Awake()
    {
        Init();
    }

    void Start()
    {
        
    }

    void Update()
    {
        Vector3 pos = transform.position;
        cameraTf.position = new Vector3(pos.x, pos.y, -10);
    }

    /// <summary>
    /// 前方に加速(移動)
    /// </summary>
    public void Move(float force)
    {
        rb.AddForce(force * moveSpeed * MOVE_POWER * transform.up);
    }

    /// <summary>
    /// 指定の向きに変更
    /// </summary>
    void ChangeDirection(Vector2 dire)
    {
        transform.rotation = Quaternion.FromToRotation(Vector2.up, dire);
    }

    /// <summary>
    /// 罠を設置
    /// </summary>
    void Trap()
    {

    }

    #region #外部から使う関数

    /// <summary>
    /// 移動距離の倍率を指定
    /// </summary>
    /// <param name="speedRatio"></param>
    public void SetMoveSpeedRatio(float speedRatio = 1)
    {
        moveSpeed = speedRatio;
    }


    #endregion

    #region #入力関数

    /// <summary>
    /// 移動入力
    /// </summary>
    /// <param name="context"></param>
    public void OnMove(InputAction.CallbackContext context)
    {
        //ボタンが押されたときだけ移動
        if(context.canceled) moveGage.StopBar();
    }
    
    /// <summary>
    /// 罠設置入力
    /// </summary>
    /// <param name="context"></param>
    public void OnTrap(InputAction.CallbackContext context)
    {
        ////ボタンが押されたとき
        if (context.started) { }
        //context.canceled
    }

    /// <summary>
    /// 向き入力
    /// </summary>
    /// <param name="context"></param>
    public void OnDirection(InputAction.CallbackContext context)
    {
        //入力中なら自身の向きを入力方向に変更
        if (context.performed)
        {
            Vector2 dire = context.ReadValue<Vector2>();
            ChangeDirection(dire);
        }
    }

    #endregion
}
