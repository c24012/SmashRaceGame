using System;
using System.Collections;
using System.Collections.Generic;
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
    [SerializeField, Tooltip("バフリスト")] List<string> effectNameList = new();

    const float MOVE_POWER = 500;
    CorseCheck.EAttribute road = CorseCheck.EAttribute.None;
    float locketSpeed = 10;
    float locketMaxSpeed = 10;

    [SerializeField, Header("フラグ確認用")] 
    bool isMove = false;                        //動いているか
    [SerializeField] bool isStop = false;       //行動不能
    [SerializeField] bool isStart = false;      //レースが始まっているか
    [SerializeField] bool isFinish = false;     //レースが終わっているか
    [SerializeField] bool isSlow = false;       //泥踏み状態
    [SerializeField] bool isSlip = false;       //滑り状態
    [SerializeField] int confusionNum = 0;      //混乱状態
    [SerializeField] bool isLocket = false;     //ロケット状態

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

        //滑っているときのチャージをキャンセル
        if (isSlip && isMove)
        {
            //パワーゲージ非表示
            powerGageCanvas.enabled = false;
            //パワーゲージのリセット
            pm.powerGage.ResetCharge();
        }
    }

    private void FixedUpdate()
    {
        //地面によって摩擦を変化
        ChangeRigidBodyDrag();

        //ロケット状態だと加速し続ける
        if (isLocket) LocketDash();
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
    /// レース終了
    /// </summary>
    public void FinishRace()
    {
        isFinish = true;
    }

    /// <summary>
    /// 指定の向きに変更
    /// </summary>
    void ChangeDirection(Vector2 dire)
    {
        transform.rotation = Quaternion.FromToRotation(Vector2.up, dire);
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

        //スリップ状態だったら落下だけ判定
        if (isSlip && road != CorseCheck.EAttribute.Out) return;

        //状態によって摩擦力を増減
        if (road == CorseCheck.EAttribute.Road) rb.drag = 3;
        else if (road == CorseCheck.EAttribute.Dart) rb.drag = 6;
        else if (road == CorseCheck.EAttribute.Warning) rb.drag = 6;
        else if (road == CorseCheck.EAttribute.RoughRoad) rb.drag = 4.5f;
        else if (road == CorseCheck.EAttribute.Out)
        {
            rb.drag = 100;
            StartCoroutine(CorseOut());
            return;
        }

        //泥踏み状態は固定
        if (isSlow) rb.drag = 6;

        //ロケット状態なら固定
        if (isLocket) rb.drag = 3;
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

    #region 状態異常関数

    /// <summary>
    /// スタン_電撃(行動不能状態)
    /// </summary>
    void Stun_ElectricShock(bool isActive)
    {
        isStop = isActive;
        if (isStop)
        {
            Init();
            pm.trap.ResetCharge();
        }
    }

    /// <summary>
    /// 滑る
    /// </summary>
    void Slip(bool isActive)
    {
        isSlip = isActive;
        if (isSlip)
        {
            rb.drag = 0.3f;
        }
    }

    /// <summary>
    /// 混乱(操作反転)
    /// </summary>
    void Confusion(bool isActive)
    {
        if (isActive) confusionNum = UnityEngine.Random.Range(1, 4);
        else confusionNum = 0;
    }

    /// <summary>
    /// ロケットダッシュ
    /// </summary>
    void LocketDash()
    {
        //前方に加速
        rb.AddForce(transform.up * locketSpeed);

        if (rb.velocity.sqrMagnitude > Mathf.Pow(locketMaxSpeed,2))
        {
            rb.velocity = rb.velocity.normalized * locketMaxSpeed;
        }
    }

    /// <summary>
    /// スタン_炎(行動不能状態)
    /// </summary>
    void Stun_Flame(bool isActive)
    {
        isStop = isActive;
        if (isStop)
        {
            Init();
            pm.trap.ResetCharge();
        }
    }

    #endregion

    #region #トラップ用関数

    /// <summary>
    /// 移動力の倍率を指定&解除
    /// </summary>
    /// <param name="speedFluctuation">増減させる値</param>
    /// <param name="trapName">トラップの名前</param>
    public void EffectMoveSpeedRatio(float speedFluctuation, bool isActive, string trapName, float chargeSpeed = 0)
    {
        //付与
        if (isActive)
        {
            //もうすでに存在している効果かどうかを取得
            bool isContain = effectNameList.Contains(trapName);
            //効果名を登録
            effectNameList.Add(trapName);
            //もとから存在していた場合返却
            if (isContain) return;
            //初めての効果の場合付与
            moveSpeedRatio += speedFluctuation;

            //加速の時は溜める速度を変更
            if (chargeSpeed != 0) pm.powerGage.AdditionChargeSpeed(chargeSpeed);
            //泥の減速中はフラグをオン
            if (speedFluctuation < 0) isSlow = true;
        }
        //解除
        else
        {
            //一つ登録から消す
            effectNameList.Remove(trapName);
            //消してもなお残っている場合返却
            if (effectNameList.Contains(trapName)) return;
            //もう残っていない効果の場合戻す
            moveSpeedRatio -= speedFluctuation;

            //溜める速度を変更
            if (chargeSpeed != 0) pm.powerGage.AdditionChargeSpeed(-chargeSpeed);
            //泥の減速中はフラグをオフ
            if (speedFluctuation < 0) isSlow = false;
        }
    }

    /// <summary>
    /// 電撃スタン付与&解除
    /// </summary>
    /// <param name="stunTime"></param>
    /// <param name="trapName"></param>
    public void EffectStun_ElectricShock(bool isActive, string trapName)
    {
        //付与
        if (isActive)
        {
            //もうすでに存在している効果かどうかを取得
            bool isContain = effectNameList.Contains(trapName);
            //効果名を登録
            effectNameList.Add(trapName);
            //もとから存在していた場合返却
            if (isContain) return;
            //初めての効果の場合は付与
            Stun_ElectricShock(true);
        }
        //解除
        else
        {
            //一つ登録から消す
            effectNameList.Remove(trapName);
            //消してもなお残っている場合返却
            if (effectNameList.Contains(trapName)) return;
            //もう残っていない効果の場合戻す
            Stun_ElectricShock(false);
        }
    }

    /// <summary>
    /// スリップ付与&解除
    /// </summary>
    /// <param name="isActive"></param>
    /// <param name="trapName"></param>
    public void EffectSlip(bool isActive,string trapName)
    {
        //付与
        if (isActive)
        {
            //もうすでに存在している効果かどうかを取得
            bool isContain = effectNameList.Contains(trapName);
            //効果名を登録
            effectNameList.Add(trapName);
            //もとから存在していた場合返却
            if (isContain) return;
            //初めての効果の場合は付与
            Slip(true);
        }
        //解除
        else
        {
            //一つ登録から消す
            effectNameList.Remove(trapName);
            //消してもなお残っている場合返却
            if (effectNameList.Contains(trapName)) return;
            //もう残っていない効果の場合戻す
            Slip(false);
        }
    }

    /// <summary>
    /// 混乱付与&解除
    /// </summary>
    /// <param name="isActive"></param>
    /// <param name="trapName"></param>
    public void EffectConfusion(bool isActive,string trapName)
    {
        //付与
        if (isActive)
        {
            //もうすでに存在している効果かどうかを取得
            bool isContain = effectNameList.Contains(trapName);
            //効果名を登録
            effectNameList.Add(trapName);
            //もとから存在していた場合返却
            if (isContain) return;
            //初めての効果の場合は付与
            Confusion(true);
        }
        //解除
        else
        {
            //一つ登録から消す
            effectNameList.Remove(trapName);
            //消してもなお残っている場合返却
            if (effectNameList.Contains(trapName)) return;
            //もう残っていない効果の場合戻す
            Confusion(false);
        }
    }

    /// <summary>
    /// 衝撃波の加速
    /// </summary>
    /// <param name="pos"></param>
    public void EffectShockWave(Vector2 pos, bool mySelf = false)
    {
        //位置の差を計算
        Vector2 vec = (Vector2)transform.position - pos;
        float diff = Mathf.Clamp(vec.magnitude, 0.5f, 2f);
        //方向の正規化
        vec.Normalize();
        //中心から離れるほど弱く加速
        //自分に当たった場合は影響を弱くする
        if (mySelf) rb.AddForce((vec * MOVE_POWER) / (diff * 2));
        else rb.AddForce((vec * MOVE_POWER) / diff);
    }

    /// <summary>
    /// ロケットダッシュ開始＆終了
    /// </summary>
    /// <param name="isActive"></param>
    public void EffectLocketDash(bool isActive,string trapName,float speed = 0, float maxSpeed = 0)
    {
        isLocket = isActive;

        //速度を指定
        locketSpeed = speed;
        //最高速度を指定
        locketMaxSpeed = maxSpeed;

        if (isActive)
        {
            //効果名を登録
            effectNameList.Add(trapName);
        }
        else
        {
            //効果名を削除
            effectNameList.Remove(trapName);
        }

        //パワーゲージ非表示
        powerGageCanvas.enabled = false;
        //パワーゲージのリセット
        pm.powerGage.ResetCharge();
    }

    /// <summary>
    /// 電撃スタン付与&解除
    /// </summary>
    /// <param name="stunTime"></param>
    /// <param name="trapName"></param>
    public void EffectStun_Flame(bool isActive, string trapName)
    {
        //付与
        if (isActive)
        {
            //もうすでに存在している効果かどうかを取得
            bool isContain = effectNameList.Contains(trapName);
            //効果名を登録
            effectNameList.Add(trapName);
            //もとから存在していた場合返却
            if (isContain) return;
            //初めての効果の場合は付与
            Stun_Flame(true);
        }
        //解除
        else
        {
            //一つ登録から消す
            effectNameList.Remove(trapName);
            //消してもなお残っている場合返却
            if (effectNameList.Contains(trapName)) return;
            //もう残っていない効果の場合戻す
            Stun_Flame(false);
        }
    }

    #endregion

    #region #入力関数

    /// <summary>
    /// 移動入力
    /// </summary>
    /// <param name="context"></param>
    public void OnMove(InputAction.CallbackContext context)
    {
        //試合終了時は返却
        if (isFinish) return;

        //行動不能時は返却
        if (isStop) return;

        //滑っていて動いている時はチャージできない
        if (isSlip && isMove) return;

        //ロケット状態ではチャージできない
        if (isLocket) return;

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
        //試合終了時は返却
        if (isFinish) return;

        //レース開始前は返却
        if (!isStart) return;
        //行動不能時は返却
        if (isStop) return;

        //ロケット状態ではチャージできない
        if (isLocket) return;

        //トラップがクールタイム中は返却
        if (!pm.trap.trapFlag) return;

        //速攻発動トラップの場合
        if (pm.trap.GetIsInstantActive())
        {
            //ボタンが押されたら発動
            if (context.started)
            {
                pm.trap.Trap();
            }
        }
        else
        {
            //ボタンが押されたら力をチャージ
            if (context.started)
            {
                pm.trap.StartCharge();
            }

            //ボタンが離されたときに発動
            if (context.canceled)
            {
                pm.trap.StopCharge();
            }
        }
    }

    /// <summary>
    /// 向き入力
    /// </summary>
    /// <param name="context"></param>
    public void OnDirection(InputAction.CallbackContext context)
    {
        //試合終了時は返却
        if (isFinish) return;

        //行動不能時は返却
        if (isStop) return;
        //入力中なら自身の向きを入力方向に変更
        if (context.performed)
        {
            Vector2 dire = context.ReadValue<Vector2>();

            //もし混乱しているなら入力を回転させる
            switch (confusionNum)
            {
                case 1: //90度回転
                    dire = new Vector2(-dire.y, dire.x);
                    break;
                case 2: //180度回転
                    dire = new Vector2(-dire.x, -dire.y);
                    break;
                case 3: //270度回転
                    dire = new Vector2(dire.y, -dire.x);
                    break;
                default:
                    break;
            }

            ChangeDirection(dire);
        }
    }

    /// <summary>
    /// 罠変更の入力
    /// </summary>
    /// <param name="context"></param>
    public void OnChange(InputAction.CallbackContext context)
    {
        //試合終了時は返却
        if (isFinish) return;

        //行動不能時は返却
        if (isStop) return;
        
        if (context.started)
        {
            float value = context.ReadValue<float>();
            if(value > 0) pm.trap.TrapChange(value: 1);
            else pm.trap.TrapChange(value: -1);
        }
    }

    #endregion
}
