using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyPlayerController : PlayerController
{
    public IllutionTrapSc illutionTrap;

    public override void EffectStun_ElectricShock(bool isActive, string trapName)
    {
        if (isActive) illutionTrap.TimeUp();
    }

    public override void EffectStun_Flame(bool isActive, string trapName)
    {
        if (isActive) illutionTrap.TimeUp();
    }

    protected override IEnumerator CorseOut()
    {
        //一旦行動不能に
        isStop = true;
        isFall = true;
        col.enabled = false;
        //初期化
        Init();
        //当たり判定をオフに
        rb.isKinematic = true;
        anim.SetTrigger("Fall");
        WaitForSeconds wait = new(0.1f);
        Quaternion deforeRotate = transform.rotation;
        audioSource.PlayOneShot(audioClip[1]);
        for (float i = 1; i > 0; i -= 0.1f)
        {
            transform.localScale = new Vector2(i, i);
            transform.Rotate(new Vector3(0, 0, 10));
            yield return wait;
        }
        cottonSr.enabled = false;
        sr.enabled = false;
        //落下中は見た目を非表示
        transform.localScale = new Vector2(1, 1);
        transform.rotation = deforeRotate;

        illutionTrap.TimeUp();
    }
}
