using System.Collections;
using UnityEngine;

public class DecelerationTrapSc : TrapThrow
{
    [SerializeField, Header("スピードの減速値")] float downSpeed = 0.5f;
    [Header("消える為にかかる時間")] public float timeItTakesToClear = 0.2f;

    override protected void LandedTrap()
    {
        //指定時間後にトラップを破壊
        Invoke(nameof(TimeUp), effectTime[rankingPower]);
        //バトルの場合マスクをかける
        if(pm.nowMode == PlayerManager.GameMode.Battle)
        {
            trapObj.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //合ったったプレイヤーのマネージャーを取得しておく
            pm = collision.transform.parent.GetComponent<PlayerManager>();
            //減速を与える
            pm.playerController.EffectMoveSpeedRatio(downSpeed, true, gameObject.name);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //合ったったプレイヤーのマネージャーを取得しておく
            pm = collision.transform.parent.GetComponent<PlayerManager>();
            //減速をリセット
            if (pm.playerController != null)
                pm.playerController.EffectMoveSpeedRatio(downSpeed, false, gameObject.name);
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
