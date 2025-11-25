using System.Collections;
using UnityEngine;

public class DecelerationTrapSc : TrapThrow
{
    [SerializeField, Header("スピードの減速値")] float downSpeed = 0.5f;

    override protected void LandedTrap()
    {
        //指定時間後にトラップを破壊
        Invoke(nameof(TimeUp), effectTime[rankingPower]);
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
            pm.playerController.EffectMoveSpeedRatio(downSpeed, false, gameObject.name);
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
