using System.Collections;
using UnityEngine;

public class SlipTrapSc : TrapBase
{
    [Header("壊れるまでの時間")] public float timeItTakesToBreak = 8f;

    private void Start()
    {
        //指定時間後にトラップを破壊
        Invoke(nameof(TimeUp),timeItTakesToBreak);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //合ったったプレイヤーのマネージャーを取得しておく
            pm = collision.transform.parent.GetComponent<PlayerManager>();
            //当たったプレイヤーに効果を付与
            pm.playerController.EffectSlip(true, gameObject.name);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //合ったったプレイヤーのマネージャーを取得しておく
            pm = collision.transform.parent.GetComponent<PlayerManager>();
            //当たったプレイヤーに効果を付与
            pm.playerController.EffectSlip(false, gameObject.name);
        }
    }

    /// <summary>
    /// トラップを破壊する
    /// </summary>
    private void TimeUp()
    {
        //トラップを破壊
        Destroy(gameObject);
    }
}
