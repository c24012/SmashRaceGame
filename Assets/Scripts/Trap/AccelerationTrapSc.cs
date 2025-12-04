using System.Collections;
using UnityEngine;

public class AccelerationTrapSc : TrapBase
{

    [SerializeField,Header("スピードの加算")] float speeds = 0.5f;
    [SerializeField,Header("チャージスピードの加算")] float chargeSpeeds = 0.5f;
    [SerializeField, Header("効果音")] AudioSource audioSource;

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
        pm.playerController.EffectMoveSpeedRatio(speeds, true, gameObject.name, chargeSpeeds);
        //各順位の時間待機
        yield return new WaitForSeconds(effectTime[rankingPower]);
        //加速をリセット
        pm.playerController.EffectMoveSpeedRatio(speeds, false, gameObject.name, chargeSpeeds);
        //音を徐々に小さくする
        WaitForSeconds wait = new(0.1f);
        for (float i = 1; i > 0; i -= 0.1f)
        {
            audioSource.volume = i;
            yield return wait;
        }
        //オブジェクトを破壊
        Destroy(gameObject);
    }
}
