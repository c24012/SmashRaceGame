using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class ConfusionTrapSc : TrapThrow
{
    [Header("消える為にかかる時間")] public float timeItTakesToClear = 0.2f;
    [Header("壊れるまでの時間")] public float timeItTakesToBreak = 0.2f;

    protected override void LandedTrap()
    {
        //指定時間後にトラップを破壊
        Invoke(nameof(TimeUp), timeItTakesToBreak);
    } 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //合ったったプレイヤーのマネージャーを取得しておく
            pm = collision.transform.parent.GetComponent<PlayerManager>();
            //当たったプレイヤーに効果を付与
            StartCoroutine(GiveEffect(pm));
        }
    }

    /// <summary>
    /// 効果を付与
    /// </summary>
    /// <param name="pm"></param>
    /// <returns></returns>
    IEnumerator GiveEffect(PlayerManager pm)
    {
        //順位を取得
        rankingPower = pm.playerData.ranking;
        //スタンを付与
        pm.playerController.EffectConfusion(true,gameObject.name);
        //解除まで待機
        yield return new WaitForSeconds(effectTime[rankingPower]);
        //スタンを解除
        pm.playerController.EffectConfusion(false, gameObject.name);
    }

    /// <summary>
    /// トラップを破壊する
    /// </summary>
    private void TimeUp()
    {
        //ゆっくり消す
        StartCoroutine(FadeAway(timeItTakesToClear));
        //効果終了後に破壊
        float time = Mathf.Max(effectTime);
        Destroy(gameObject, time + 1);
    }

    /// <summary>
    /// ゆっくり消える
    /// </summary>
    IEnumerator FadeAway(float speedTime)
    {
        SpriteRenderer sr = trapObj.GetComponent<SpriteRenderer>();
        Collider2D col = trapObj.GetComponent<Collider2D>();
        Color32 color = sr.color;

        WaitForSeconds wait = new(speedTime / 5);  //１ループで待つ時間
        byte reducAlpha = (byte)(color.a / 5);     //１ループで消える割合
        col.enabled = false;                        //判定を先に消す

        //だんだん透明に
        for (int i = 0; i < 5; i++)
        {
            yield return wait;
            color.a -= reducAlpha;
            sr.color = color;
        }
        //スプライト自体を非表示
        sr.enabled = false;
    }
}
