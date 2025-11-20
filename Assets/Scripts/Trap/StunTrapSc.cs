using System.Collections;
using UnityEngine;

public class StunTrapSc : TrapBase
{
    [Header("壊れるまでの時間")] public float timeItTakesToBreak = 0.2f;

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
        pm.playerController.EffectStun_ElectricShock(true,gameObject.name);
        //解除まで待機
        yield return new WaitForSeconds(effectTime[rankingPower]);
        //スタンを解除
        pm.playerController.EffectStun_ElectricShock(false, gameObject.name);
    }

    /// <summary>
    /// トラップを破壊する
    /// </summary>
    private void TimeUp()
    {
        //先に当たり判定を消す
        col.enabled = false;
        sr.enabled = false;
        //効果終了後に破壊
        float time = Mathf.Max(effectTime);
        Destroy(gameObject, time + 1);
    }
}
