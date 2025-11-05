using System;
using System.Drawing;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    MoveGage moveGage;
    [SerializeField] CorseChack corseChack;

    Rigidbody2D rb;
    [SerializeField] GameObject myCamera;
    [SerializeField] Animator anim;

    [SerializeField] float moveSpeedRatio = 1;
    CorseChack.EAttribute road = CorseChack.EAttribute.None;
    [SerializeField] int cameraZoom = 0;
    [SerializeField] int mapSizeWeight_test = 480;
    [SerializeField] int mapSizeHeight_test = 270;

    const float MOVE_POWER = 500;

    [SerializeField] float moveSpeed = 1;
    [SerializeField] bool isMove = false;
    
    //トラップの変数
    [Header("生成するトラップ")]
    public GameObject[] trapObj;

    [Header("置くトラップの種類の番号")]
    public int trapNum = 0;

    [Header("Rayの長さ")]
    public float length;

    PlayerManager playerManager;

    /// <summary>
    /// 初期化
    /// </summary>
    void Init()
    {
        rb = GetComponent<Rigidbody2D>();
        moveGage = GetComponent<MoveGage>();

        playerManager = GetComponent<PlayerManager>();
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
        CameraTracking();
        CheckIsMove();
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
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, length, 1);

        //壁に当たったか、当たってないか
        if (hit.collider != null)
        {

            Instantiate(trapObj[trapNum], hit.point, Quaternion.identity).
                GetComponent<TrapSc>().trapNum = playerManager.playerNum;
        }
        else
        {
            Instantiate(trapObj[trapNum], transform.position + transform.up * length, Quaternion.identity).
                GetComponent<TrapSc>().trapNum = playerManager.playerNum;
        }
    }

    /// <summary>
    /// カメラがプレイヤーを追尾
    /// </summary>
    void CameraTracking()
    {
        Vector3 pos = transform.position;
        Vector2 clampSize = Vector2.zero;
        switch (cameraZoom)
        {
            case 1:
                clampSize = new Vector2(mapSizeWeight_test * 4 - 1280, mapSizeHeight_test * 4 - 720);
                break;
            case 2:
                clampSize = new Vector2(mapSizeWeight_test * 4 - 864, mapSizeHeight_test * 4 - 486);
                break;
            case 3:
                clampSize = new Vector2(mapSizeWeight_test * 4 - 448, mapSizeHeight_test * 4 - 252);
                break;
        }
        pos = new Vector3(
            Mathf.Clamp(pos.x, -clampSize.x / 200, clampSize.x / 200),
            Mathf.Clamp(pos.y, -clampSize.y / 200, clampSize.y / 200),
            myCamera.transform.position.z);
        myCamera.transform.position = pos;
    }

    /// <summary>
    /// 動いているか時のアニメーション処理
    /// </summary>
    void CheckIsMove()
    {
        //速度がx,yいずれかが0.3以上だと「動いている」判定
        isMove = Mathf.Abs(rb.velocity.x) > 0.3f || Mathf.Abs(rb.velocity.y) > 0.3f;
        //動いているかをアニメーションに適応
        if (isMove)
        {
            anim.SetBool("IsMove",true);
        }
        else
        {
            anim.SetBool("IsMove", false);
        }
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
        if (context.canceled)
        {
            Trap();
        }
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

    /// <summary>
    /// カメラズーム入力
    /// </summary>
    /// <param name="context"></param>
    public void OnZoom(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            float value = context.ReadValue<float>();
            cameraZoom = Mathf.Clamp(cameraZoom + (int)value, 0, 3);

            float size = 5.4f;
            if (cameraZoom == 0) size = mapSizeHeight_test * 2 / 100f;
            else if (cameraZoom == 1) size = 3.6f;
            else if (cameraZoom == 2) size = 2.43f;
            else if (cameraZoom == 3) size = 1.26f;
            myCamera.GetComponent<Camera>().orthographicSize = size;
        } 
    }
    #endregion
}
