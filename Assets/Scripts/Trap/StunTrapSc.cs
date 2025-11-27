using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class StunTrapSc : TrapBase
{
    [SerializeField] PlayableDirector anim;
    [SerializeField] GameObject warningAreaObj;
    Collider2D trapCol;

    private void Awake()
    {
        //アニメーションコンポーネントを取得
        trapCol = warningAreaObj.GetComponent<Collider2D>();

        transform.rotation = Quaternion.identity;

        anim.stopped += TimeUp;
        anim.Play();
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

    public void SetColliderEnable(bool isEnable)
    {
        trapCol.enabled = isEnable;
    }

    /// <summary>
    /// トラップを破壊する
    /// </summary>
    private void TimeUp(PlayableDirector director)
    {
        anim.stopped -= TimeUp;
        float maxTime = Mathf.Max(effectTime);
        Destroy(gameObject, maxTime);
    }
}
