using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocketTrapSc : TrapBase
{
    [SerializeField, Header("移動速度")] float speed = 10f;
    [SerializeField, Header("最高速度")] float maxSpeed = 10f;

    private void Start()
    {
        //効果を付与
        StartCoroutine(GiveEffect(pm));
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
        //加速を与える
        pm.playerController.EffectLocketDash(true, gameObject.name, speed, maxSpeed);
        //各順位の時間待機
        yield return new WaitForSeconds(effectTime[rankingPower]);
        //加速をリセット
        pm.playerController.EffectLocketDash(false, gameObject.name);
        //オブジェクトを破壊
        Destroy(gameObject);
    }
}
