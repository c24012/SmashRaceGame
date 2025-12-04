using System.Collections;
using UnityEngine;

public class LocketTrapSc : TrapBase
{
    [SerializeField, Header("移動速度")] float speed = 10f;
    [SerializeField, Header("最高速度")] float maxSpeed = 10f;
    [SerializeField, Header("効果音")] AudioSource audioSource;

    float timer = 0;

    private void Start()
    {
        //効果を付与
        GiveEffect();
        //制限時間をランキングに合わせて設定
        timer = effectTime[rankingPower];
    }

    private void FixedUpdate()
    {
        //１位になった瞬間効果時間を大幅減少
        if(pm.playerData.ranking == 0)
        {
            if(timer > effectTime[0])
            {
                timer = effectTime[0];
            }
        }

        //タイマーの値1/sを減らしていく
        timer -= Time.fixedDeltaTime;
        if(timer <= 0)
        {
            RemoveEffect();
            timer = 10000;
        }
    }

    /// <summary>
    /// 効果を付与
    /// </summary>
    /// <param name="pm"></param>
    /// <returns></returns>
    void GiveEffect()
    {
        //順位を取得
        rankingPower = pm.playerData.ranking;
        //加速を与える
        pm.playerController.EffectLocketDash(true, gameObject.name, speed, maxSpeed);
    }

    void RemoveEffect()
    {
        //加速をリセット
        pm.playerController.EffectLocketDash(false, gameObject.name);
        //音を徐々に小さくする
        for (float i = 1; i > 0; i -= 0.1f)
        {
            audioSource.volume = i;
        }
        //オブジェクトを破壊
        Destroy(gameObject);
    }
}
