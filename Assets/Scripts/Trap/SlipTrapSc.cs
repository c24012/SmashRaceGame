using System.Collections;
using UnityEngine;

public class SlipTrapSc : TrapThrow
{
    [Header("壊れるまでの時間")] public float timeItTakesToBreak = 8f;
    [Header("消える為にかかる時間")] public float timeItTakesToClear = 0.1f;

    override protected void LandedTrap()
    {
        //指定時間後にトラップを破壊
        Invoke(nameof(TimeUp),timeItTakesToBreak);
        //バトルの場合マスクをかける
        if (pm.nowMode == PlayerManager.GameMode.Battle)
        {
            trapObj.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //当たったプレイヤーのマネージャーを取得しておく
            pm = collision.transform.parent.GetComponent<PlayerManager>();
            //当たったプレイヤーに効果を付与
            pm.playerController.EffectSlip(true, gameObject.name);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //抜けたプレイヤーのマネージャーを取得しておく
            pm = collision.transform.parent.GetComponent<PlayerManager>();
            //抜けたプレイヤーは効果を解除
            if (pm.playerController != null)
                pm.playerController.EffectSlip(false, gameObject.name);
        }
    }

    /// <summary>
    /// トラップを破壊する
    /// </summary>
    private void TimeUp()
    {
        StartCoroutine(FadeAway(timeItTakesToClear));
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
        //トラップ自体を破壊
        Destroy(gameObject);
    }
}
