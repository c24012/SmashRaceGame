using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    PlayerManager pm;

    Rigidbody2D rb;
    [SerializeField] Transform guisTf;
    [SerializeField] Canvas canvas;
    Animator anim;
    [SerializeField, Tooltip("生成するトラップ")] GameObject[] trapObj;

    [SerializeField, Tooltip("速度の倍率")] float moveSpeedRatio = 1;
    [SerializeField, Tooltip("Rayの長さ")] float length;
    [SerializeField, Tooltip("置くトラップの種類の番号")] int trapNum = 0;

    const float MOVE_POWER = 500;
    CorseCheck.EAttribute road = CorseCheck.EAttribute.None;

    [SerializeField,Header("フラグ確認用")] bool trapFlag = true;
    [SerializeField] bool isMove = false;

    /// <summary>
    /// 初期化
    /// </summary>
    void Init()
    {
        //プレイヤーマネージャー取得
        pm = GetComponent<PlayerManager>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        //パワーゲージ非表示
        canvas.enabled = false;
    }

    private void Awake()
    {
        Init();
    }

    void Update()
    {
        GUIsTracking();
        CheckIsMove();
    }

    private void FixedUpdate()
    {
        ChangeRigidBodyDrag();
    }

    /// <summary>
    /// 前方に加速(移動)
    /// </summary>
    public void Move(float power)
    {
        //進む力を計算
        float force = power * moveSpeedRatio * MOVE_POWER;
        //ダート判定の時は減速
        if(road == CorseCheck.EAttribute.Dart)
        {
            force *= 0.3f;
        }
        else if (road == CorseCheck.EAttribute.RoughRoad)
        {
            force *= 0.7f;
        }
        else if (road == CorseCheck.EAttribute.Warning)
        {
            force *= 0.3f;
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
        bool isUp = trapObj[trapNum].GetComponent<TrapSc>().trapType == TrapBase.ETrapType.Up;
        if (trapFlag)
        {
            trapFlag = false;
            Invoke(new Action(() => { trapFlag = true; }).Method.Name, 3);
        }
        else
        {
            return;
        }
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up * (isUp ? 1:-1), length, 1) ;

        //壁に当たったか、当たってないか
        if (hit.collider != null)
        {

            Instantiate(trapObj[trapNum], hit.point, Quaternion.identity).
                GetComponent<TrapSc>().trapNum = pm.playerNum;
        }
        else
        {
            Instantiate(trapObj[trapNum], transform.position + (transform.up * (isUp ? 1 : -1)) * length, Quaternion.identity).
                GetComponent<TrapSc>().trapNum = pm.playerNum;
        }
    }

    /// <summary>
    /// GUIキャンバスをプレイヤーに追尾
    /// </summary>
    void GUIsTracking()
    {
        guisTf.position = transform.position;
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

    /// <summary>
    /// 道の状態を取得＆摩擦力を増減
    /// </summary>
    void ChangeRigidBodyDrag()
    {
        //現在の道の状態を確認＆取得
        road = pm.corseCheck.GetAttribute(transform.position);
        //状態によって摩擦力を増減
        if (road == CorseCheck.EAttribute.Road) rb.drag = 3;
        else if (road == CorseCheck.EAttribute.Dart) rb.drag = 5;
        else if (road == CorseCheck.EAttribute.Warning) rb.drag = 5;
        else if (road == CorseCheck.EAttribute.RoughRoad) rb.drag = 4f;
        else if (road == CorseCheck.EAttribute.Out) rb.drag = 100;
    }

    #region #外部から使う関数

    /// <summary>
    /// 移動距離の倍率を指定
    /// </summary>
    /// <param name="speedRatio"></param>
    public void SetMoveSpeedRatio(float speedFluctuation = 1)
    {
        moveSpeedRatio += speedFluctuation;
    }


    #endregion

    #region #入力関数

    /// <summary>
    /// 移動入力
    /// </summary>
    /// <param name="context"></param>
    public void OnMove(InputAction.CallbackContext context)
    {
        //ボタンが押されたら力をチャージ
        if (context.started)
        {
            pm.powerGage.StartCharge();
            canvas.enabled = true;
        }
            
        //ボタンが離されたときに移動
        if(context.canceled)
        {
            pm.powerGage.StopCharge();
            canvas.enabled = false;
        }
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
    public void OnChange(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            float value = context.ReadValue<float>();
            if (trapNum + value < 0)
            {
                trapNum = trapObj.Length;
            }
            else if (trapNum + value > trapObj.Length)
            {
                trapNum = 0;
            }
            else
            {
                trapNum += (int)value;
            }
        }
    }
    #endregion
}
