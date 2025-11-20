using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseTrapSc : TrapBase
{
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Collider2D sensorCol;
    [SerializeField] Collider2D bodyCol;
    [SerializeField] SpriteRenderer chaseSr;
    [Header("加速度")] public float acceleration = 20;
    [Header("最高速度")] public float maxSpeed = 20;
    [Header("追尾性能")] public float chasePerformance;
    [Header("壊れるまでの時間")] public float timeItTakesToBreak = 20;

    float speed = 0;

    private void Awake()
    {
        //初速度は0
        speed = 0;
    }

    private void Start()
    {
        //指定時間後にトラップを破壊
        Invoke(nameof(TimeUp), timeItTakesToBreak);
    }


    private void FixedUpdate()
    {
        //加速していく
        speed += Time.fixedDeltaTime * acceleration;
        //最大速度を上限にする
        speed = Mathf.Min(speed, maxSpeed);
        //進ませる
        rb.velocity = transform.up * speed;
    }

    /// <summary>
    /// 対象のtransformを参照して追尾する
    /// </summary>
    /// <param name="targetTf"></param>
    public void ChasingPlayer(Transform targetTf)
    {
        //対象の方向を計算
        Vector3 targetVec = targetTf.position - transform.position;
        //角度差を取得
        float angle = Vector2.SignedAngle(transform.up, targetVec);
        //少しづつ角度を変える
        transform.Rotate(0f, 0f, angle * chasePerformance * Time.deltaTime);
    }

    /// <summary>
    /// 当たった相手に効果を付与
    /// </summary>
    /// <param name="collision"></param>
    public IEnumerator GiveEffect(PlayerManager pm)
    {
        //破壊を予約
        TimeUp();
        //順位を取得
        rankingPower = pm.playerData.ranking;
        //付与
        pm.playerController.EffectStun_Flame(true, gameObject.name);
        //順位によって一定時間待機
        yield return new WaitForSeconds(effectTime[rankingPower]);
        //解除
        pm.playerController.EffectStun_Flame(false, gameObject.name);
    }

    /// <summary>
    /// トラップを破壊する
    /// </summary>
    public void TimeUp()
    {
        //先に判定を消す
        sensorCol.enabled = false;
        bodyCol.enabled = false;
        chaseSr.enabled = false;
        //効果終了を待って破壊
        float time = Mathf.Max(effectTime);
        Destroy(gameObject, time + 1);
    }
}
