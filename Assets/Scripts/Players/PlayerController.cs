using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    PlayerManager pm;

    Rigidbody2D rb;
    Animator anim;
    SpriteRenderer sr;

    [SerializeField, Tooltip("プレイヤーUI"),Header("コンポーネント")] Transform guisTf;
    [SerializeField, Tooltip("パワーゲージキャンバス")] Canvas powerGageCanvas;
    [Tooltip("生成するトラップ")] public GameObject[] trapObj = new GameObject[4];

    [SerializeField, Tooltip("速度の倍率"),Header("変数")] float moveSpeedRatio = 1;
    [SerializeField, Tooltip("Rayの長さ")] float length;
    [SerializeField, Tooltip("置くトラップの種類の番号")] int trapNum = 0;
    [SerializeField, Tooltip("バフリスト")] List<string> buffNameList = new();

    const float MOVE_POWER = 500;
    CorseCheck.EAttribute road = CorseCheck.EAttribute.None;

    [SerializeField,Header("フラグ確認用")] bool trapFlag = true;
    [SerializeField] bool isMove = false;
    [SerializeField] bool isStop = false;
    [SerializeField] bool isStart = false;

    /// <summary>
    /// 初期化
    /// </summary>
    void Init()
    {
        //パワーゲージ非表示
        powerGageCanvas.enabled = false;
        //パワーゲージのリセット
        pm.powerGage.ResetCharge();
        //加速度リセット
        rb.velocity = Vector2.zero;
    }

    private void Awake()
    {
        //レース開始まで待機状態
        isStart = false;

        //プレイヤーマネージャー取得
        pm = transform.transform.parent.GetComponent<PlayerManager>();
        //その他のコンポーネントを取得
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        //初期化
        Init();
    }

    void Update()
    {
        //UIをプレイヤーに追尾
        GUIsTracking();
        //動いているとアニメーションを変化
        CheckIsMove();
    }

    private void FixedUpdate()
    {
        //地面によって摩擦を変化
        ChangeRigidBodyDrag();
    }

    /// <summary>
    /// 前方に加速(移動)
    /// </summary>
    public void Move(float power)
    {
        //レース開始前は返却
        if (!isStart) return;
        //進む力を計算
        float force = power * moveSpeedRatio * MOVE_POWER;
        //ダート判定の時は減速
        if(road == CorseCheck.EAttribute.Dart)
        {
            force *= 0.5f;
        }
        else if (road == CorseCheck.EAttribute.RoughRoad)
        {
            force *= 0.7f;
        }
        else if (road == CorseCheck.EAttribute.Warning)
        {
            force *= 0.5f;
        }
        //前方に加速
        rb.AddForce(force * transform.up);
    }

    /// <summary>
    /// レース開始
    /// </summary>
    public void StartRace()
    {
        isStart = true;
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
        bool isUp = trapObj[trapNum].GetComponent<TrapBase>().trapType == TrapBase.ETrapType.Up;
        if (trapFlag)
        {
            trapFlag = false;
            Invoke(new Action(() => { trapFlag = true; pm.iconManager.BanCheck(!trapFlag);}).Method.Name, 3);
        }
        else
        {
            return;
        }
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up * (isUp ? 1:-1), length, 1) ;
        TrapBase sampleBase = null;
        //壁に当たったか、当たってないか
        if (hit.collider != null)
        {

            sampleBase = Instantiate(trapObj[trapNum], hit.point, Quaternion.identity).
                GetComponent<TrapBase>();
        }
        else
        {
            sampleBase = Instantiate(trapObj[trapNum], 
                transform.position + (transform.up * (isUp ? 1 : -1)) * length, Quaternion.identity).
                GetComponent<TrapBase>();
        }
        sampleBase.pm = pm;
        pm.iconManager.BanCheck(!trapFlag);
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
        //行動不能時は返却
        if (isStop) return;
        //現在の道の状態を確認＆取得
        road = pm.corseCheck.GetAttribute(transform.position);
        //状態によって摩擦力を増減
        if (road == CorseCheck.EAttribute.Road) rb.drag = 3;
        else if (road == CorseCheck.EAttribute.Dart) rb.drag = 6;
        else if (road == CorseCheck.EAttribute.Warning) rb.drag = 6;
        else if (road == CorseCheck.EAttribute.RoughRoad) rb.drag = 4.5f;
        else if (road == CorseCheck.EAttribute.Out)
        {
            rb.drag = 100;
            StartCoroutine(CorseOut());
        }
    }

    /// <summary>
    /// コース外に出たときのコース復帰
    /// </summary>
    IEnumerator CorseOut()
    {
        //一旦行動不能に
        isStop = true;
        //初期化
        Init();
        //当たり判定をオフに
        rb.isKinematic = true;
        //落下アニメーション待機
        yield return new WaitForSeconds(0.5f);
        //落下中は見た目を非表示
        sr.enabled = false;
        //落下ペナルティタイム
        yield return new WaitForSeconds(1);
        //一番近くのルートへ移動
        transform.position = pm.playerData.nearestPos;
        //点滅表示
        WaitForSeconds wait = new(0.05f);
        for(int i = 0; i < 20; i++)
        {
            sr.enabled = !sr.enabled;
            yield return wait;
        }
        //全部元に戻す
        rb.isKinematic = false;
        sr.enabled = true;
        isStop = false;
    }

    #region #外部から使う関数

    /// <summary>
    /// 移動力の倍率を指定
    /// </summary>
    /// <param name="speedFluctuation">増減させる値</param>
    /// <param name="trapName">トラップの名前</param>
    public void AddMoveSpeedRatio(float speedFluctuation, string trapName)
    {
        //もうすでに存在している効果かどうかを取得
        bool isContain = buffNameList.Contains(trapName);
        //効果名を登録
        buffNameList.Add(trapName);
        //もとから存在していた場合返却
        if (isContain) return;
        //初めての効果の場合付与
        moveSpeedRatio += speedFluctuation;
    }

    /// <summary>
    /// 移動力倍率を戻す
    /// </summary>
    /// <param name="speedFluctuation">増減させた値</param>
    /// <param name="trapName">トラップの名前</param>
    public void RemoveMoveSpeedRaito(float speedFluctuation, string trapName)
    {
        //一つ登録から消す
        buffNameList.Remove(trapName);
        //消してもなお残っている場合返却
        if (buffNameList.Contains(trapName)) return;
        //もう残っていない効果の場合戻す
        else moveSpeedRatio -= speedFluctuation;
    }

    #endregion

    #region #入力関数

    /// <summary>
    /// 移動入力
    /// </summary>
    /// <param name="context"></param>
    public void OnMove(InputAction.CallbackContext context)
    {
        //行動不能時は返却
        if (isStop) return;
        //ボタンが押されたら力をチャージ
        if (context.started)
        {
            pm.powerGage.StartCharge();
            powerGageCanvas.enabled = true;
        }
            
        //ボタンが離されたときに移動
        if(context.canceled)
        {
            pm.powerGage.StopCharge();
            powerGageCanvas.enabled = false;
        }
    }
    
    /// <summary>
    /// 罠設置入力
    /// </summary>
    /// <param name="context"></param>
    public void OnTrap(InputAction.CallbackContext context)
    {
        //レース開始前は返却
        if (!isStart) return;
        //行動不能時は返却
        if (isStop) return;
        ////ボタンが離されたとき
        if (context.canceled)
        {
            Trap();
        }
    }

    /// <summary>
    /// 向き入力
    /// </summary>
    /// <param name="context"></param>
    public void OnDirection(InputAction.CallbackContext context)
    {
        //行動不能時は返却
        if (isStop) return;
        //入力中なら自身の向きを入力方向に変更
        if (context.performed)
        {
            Vector2 dire = context.ReadValue<Vector2>();
            ChangeDirection(dire);
        }
    }

    /// <summary>
    /// 罠変更の入力
    /// </summary>
    /// <param name="context"></param>
    public void OnChange(InputAction.CallbackContext context)
    {
        //行動不能時は返却
        if (isStop) return;
        if (context.started)
        {
            float value = context.ReadValue<float>();
            //0～最大値までのループ計算
            trapNum = (trapNum + (int)value + trapObj.Length) % trapObj.Length;
            //トラップとアイコンを変更
            pm.iconManager.IconChange(trapNum);
            pm.iconManager.BanCheck(!trapFlag);
        }
    }
    #endregion
}
