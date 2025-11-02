using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    MoveGage moveGage;
    [SerializeField] CorseChack corseChack;

    Rigidbody2D rb;
    [SerializeField] Transform cameraTf;

    [SerializeField] float moveSpeedRatio = 1;
    CorseChack.EAttribute road = CorseChack.EAttribute.None;

    const float MOVE_POWER = 500;

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
        cameraTf.position = new Vector3(pos.x, pos.y, cameraTf.position.z);
    }

    private void FixedUpdate()
    {
        //現在の道の状態を確認＆取得
        road = corseChack.GetAttribute(transform.position);
        //状態によって摩擦力を増減
        if (road == CorseChack.EAttribute.Road) rb.drag = 3;
        else if (road == CorseChack.EAttribute.Dart) rb.drag = 4;
    }

    /// <summary>
    /// 前方に加速(移動)
    /// </summary>
    public void Move(float power)
    {
        //進む力を計算
        float force = power * moveSpeedRatio * MOVE_POWER;
        //ダート判定の時は減速
        if(road == CorseChack.EAttribute.Dart)
        {
            force *= 0.5f;
        }
        //前方に加速
        rb.AddForce(force * transform.up);
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
        moveSpeedRatio = speedRatio;
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
