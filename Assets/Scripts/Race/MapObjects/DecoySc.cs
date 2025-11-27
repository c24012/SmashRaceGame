using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Windows;

public class DecoySc : MonoBehaviour
{
    [SerializeField, Header("コースチェック")] CorseCheck corseCheck;

    [SerializeField, Header("最大耐久度")] float strengthMax;
    [SerializeField, Header("復活時間")] float resetTime;
    [SerializeField, Header("ダメージ許容値")] float damegePermission;
    [SerializeField,Header("加速度")] float acceleration = 20;
    [SerializeField,Header("最高速度")] float maxSpeed = 20;
    [SerializeField,Header("追尾性能")] float chasePerformance;
    [SerializeField, Header("衝突時停止時間")] float stopTime;

    [SerializeField] Animator anim;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] SpriteRenderer sr;
    [SerializeField] Collider2D collisionCol;

    Vector2 firstPos;
    int enterPlayerCount = 0;
    float strength = 0;
    float speed = 0;
    Transform targetTf = null;
    bool isStop = false;
    int stopAccumulationCount = 0;
    bool isBreak = false;
    bool isOut = false;

    List<Collider2D> enterColliderList = new();

    private void Awake()
    {
        //耐久値を代入
        strength = strengthMax;
        //初期位置を登録
        firstPos = transform.position;

        rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        enterPlayerCount++;
        if (enterColliderList.Contains(collision)) return;
        enterColliderList.Add(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        enterPlayerCount--;
        if (enterColliderList.Contains(collision)) enterColliderList.Remove(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isBreak || isStop || isOut) return;
        if (collision.CompareTag("Player"))
        {
            //狙っている対象がいない場合
            if(targetTf == null)
            {
                targetTf = collision.transform;
                return;
            }

            //追跡
            ChasingPlayer();

            //狙っているプレイヤーの時は無視
            if (targetTf == collision.transform) return;
            //今入っているプレイヤーと狙っているプレイヤーの距離を計測
            float sampleDistance = (collision.transform.position - transform.position).sqrMagnitude;
            float targetDistance = (targetTf.position - transform.position).sqrMagnitude;
            //どっちが近いか比べる
            if (sampleDistance > targetDistance)
            {
                //近い方を狙う
                targetTf = collision.transform;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isBreak || isStop || isOut) return;
        if (collision.gameObject.CompareTag("Player"))
        {
            //一定以下の衝撃は無視
            if (collision.relativeVelocity.magnitude <= damegePermission) return;
            if (strength - collision.relativeVelocity.magnitude <= 0)
            {
                //破壊アニメーション
                anim.SetBool("isBreak", true);
                speed = 0;
                stopAccumulationCount = 100;
                sr.color = new Color(0.5f, 0.5f, 0.5f);
                isBreak = true;
                Invoke(nameof(ResetDecoy), resetTime);
            }
            else
            {
                //被ダメージアニメーション
                anim.SetBool("isDamage", true);
                strength -= collision.relativeVelocity.magnitude;
                //ぶつかったら一時停止
                speed = 0;
                isStop = true;
                stopAccumulationCount++;
                //移動アニメーション解除
                anim.SetBool("isWalk", false);
                //一定時間後に移動再開
                Invoke(nameof(MovingAgain), stopTime);
            }
        }
    }

    private void Update()
    {
        //途中で判定がなくなったプレイヤーをリストから削除
        foreach(Collider2D col in enterColliderList)
        {
            if(col.enabled == false)
            {
                enterColliderList.Remove(col);
            }
        }
    }

    void FixedUpdate()
    {
        //場外チェック
        if(!isOut) ChackOutCorse();
        if (isBreak || isStop || isOut) return;
        if (enterPlayerCount > 0)
        {
            //加速していく
            speed += Time.fixedDeltaTime * acceleration;
            //最大速度を上限にする
            speed = Mathf.Min(speed, maxSpeed);
            //進ませる
            rb.velocity = transform.up * speed;
            //移動アニメーション
            if (anim.GetBool("isWalk")) return;
            anim.SetBool("isWalk", true);
        }
        else
        {
            //移動アニメーション解除
            if (anim.GetBool("isWalk")) anim.SetBool("isWalk", false);
        }
    }
        
    /// <summary>
    /// 場外に入っているか
    /// </summary>
    void ChackOutCorse()
    {
        if(corseCheck.GetAttribute(transform.position) == CorseCheck.EAttribute.Out)
        {
            StartCoroutine(CorseOut());
        }
    }


    /// <summary>
    /// 対象のtransformを参照して追尾する
    /// </summary>
    /// <param name="targetTf"></param>
    void ChasingPlayer()
    {
        //対象の方向を計算
        Vector3 targetVec = targetTf.position - transform.position;
        //角度差を取得
        float angle = Vector2.SignedAngle(transform.up, targetVec);
        //少しづつ角度を変える
        transform.Rotate(0f, 0f, angle * chasePerformance * Time.deltaTime);
    }

    IEnumerator CorseOut()
    {
        //場外判定オン
        isOut = true;
        //速度をリセット
        rb.velocity = Vector2.zero;
        //当たり判定をオフに
        collisionCol.enabled = false;
        rb.isKinematic = true;
        anim.SetTrigger("Fall");
        WaitForSeconds wait = new(0.1f);
        Quaternion deforeRotate = transform.rotation;
        for (float i = 1; i > 0; i -= 0.1f)
        {
            transform.localScale = new Vector2(i, i);
            transform.Rotate(new Vector3(0, 0, 10));
            yield return wait;
        }
        sr.enabled = false;
        //落下中は見た目を非表示
        transform.localScale = new Vector2(1, 1);
        transform.rotation = deforeRotate;
        //落下ペナルティタイム
        yield return new WaitForSeconds(3f);
        //初期位置へ移動
        transform.position = firstPos;
        //点滅表示
        wait = new(0.05f);
        for (int i = 0; i < 20; i++)
        {
            sr.enabled = !sr.enabled;
            yield return wait;
        }
        //全部元に戻す
        collisionCol.enabled = true;
        rb.isKinematic = false;
        sr.enabled = true;
        ResetDecoy();
        //場外判定オフ
        isOut = false;
    }

    /// <summary>
    /// 再び動き出す
    /// </summary>
    void MovingAgain()
    {
        stopAccumulationCount--;
        if (stopAccumulationCount <= 0)
        {
            isStop = false;
            //被ダメージアニメーション解除
            anim.SetBool("isDamage", false);
        }
    }

    void ResetDecoy()
    {
        //破壊アニメーション解除
        anim.SetBool("isBreak", false);
        //耐久値を回復
        strength = strengthMax;
        //色を戻す
        sr.color = new Color(1,1,1);
        //衝撃重複値をリセット
        stopAccumulationCount = 0;
        //破壊フラグをオフ
        isBreak = false;
    }
}
