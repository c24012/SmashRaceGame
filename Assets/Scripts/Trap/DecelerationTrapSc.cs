using System.Collections;
using UnityEngine;

public class DecelerationTrapSc : TrapBase
{
    [SerializeField, Header("スピードの減速値")] float downSpeed = 0.5f;
    [Header("壊れるまでの時間")] public float timeItTakesToBreak = 10;

    private void Start()
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
            //一定時間後に破壊
            TimeUp();
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
        //減速を与える
        pm.playerController.EffectMoveSpeedRatio(downSpeed,true,gameObject.name);
        //各順位の時間待機
        yield return new WaitForSeconds(effectTime[rankingPower]);
        //減速をリセット
        pm.playerController.EffectMoveSpeedRatio(downSpeed,false,gameObject.name);
    }

    /// <summary>
    /// トラップを破壊する
    /// </summary>
    private void TimeUp()
    {
        //先に判定を消す
        col.enabled = false;
        sr.enabled = false;
        //最後の効果時間を待って破壊
        float time = Mathf.Max(effectTime);
        Destroy(gameObject, time + 1);
    }
}
