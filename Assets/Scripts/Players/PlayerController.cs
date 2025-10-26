using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    MeoshiGage meoshiGage;

    const float MOVE_POWER = 500;

    /// <summary>
    /// 初期化
    /// </summary>
    void Init()
    {
        rb = GetComponent<Rigidbody2D>();
        meoshiGage = GetComponent<MeoshiGage>();
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
    }

    /// <summary>
    /// 前方に加速(移動)
    /// </summary>
    public void Move(float force)
    {
        rb.AddForce(force * MOVE_POWER * transform.up);
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

    #region #入力関数

    /// <summary>
    /// 移動入力
    /// </summary>
    /// <param name="context"></param>
    public void OnMove(InputAction.CallbackContext context)
    {
        //ボタンが押されたときだけ移動
        //if (context.started) Move();
        if(context.canceled) meoshiGage.StopBar();
    }
    
    /// <summary>
    /// 罠設置入力
    /// </summary>
    /// <param name="context"></param>
    public void OnTrap(InputAction.CallbackContext context)
    {
        ////ボタンが押されたとき
        //if (context.started) { }
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
