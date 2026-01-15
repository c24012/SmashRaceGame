using System.Collections;
using UnityEngine;

public class TransparentTrapSc : TrapBase
{
    [SerializeField, Header("効果音")] AudioSource audioSource;

    Transform pmCharactorTf;

    private void Start()
    {
        //効果を付与
        StartCoroutine(GiveEffect(pm));
        //主キャラのtransformを取得
        pmCharactorTf = pm.transform.GetChild(0);
        //トラップの向きを初期化
        transform.rotation = Quaternion.identity;
        //SE再生
        audioSource.Play();
    }

    private void Update()
    {
        //アニメーションのためにプレイヤーの場所へ移動
        transform.position = pmCharactorTf.position;
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
        //透過を付与
        pm.playerController.EffectTransparent(true, gameObject.name);
        //各順位の時間待機
        yield return new WaitForSeconds(effectTime[rankingPower]);
        //透過を解除
        if (pm.playerController != null)
            pm.playerController.EffectTransparent(false, gameObject.name);
        //オブジェクトを破壊
        Destroy(gameObject);
    }
}
