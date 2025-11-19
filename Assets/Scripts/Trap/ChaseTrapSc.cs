using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseTrapSc : TrapBase
{
    [Header("壊れるまでの時間")] public float timeItTakesToBreak = 20;

    private void Start()
    {
        //指定時間後にトラップを破壊
        Invoke(nameof(TimeUp), timeItTakesToBreak);
    }

    private void FixedUpdate()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //合ったったプレイヤーのマネージャーを取得しておく
            pm = collision.transform.parent.GetComponent<PlayerManager>();
            //当たったプレイヤーに効果を付与
            pm.playerController.EffectShockWave(transform.position);
        }
    }

    /// <summary>
    /// トラップを破壊する
    /// </summary>
    private void TimeUp()
    {
        Destroy(gameObject);
    }
}
