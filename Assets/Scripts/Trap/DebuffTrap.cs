using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebuffTrap : TrapBase
{
    [Header("ã©‚É‚æ‚éŒ¸‘¬—¦")]
    public float downSpeed = 0.8f;

    //private void Start()
    //{
    //    trapType = ETrapType.Down;
    //}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            pm = collision.transform.parent.GetComponent<PlayerManager>();
            col.enabled = false;
            sr.enabled = false;
            pm.playerController.SetMoveSpeedRatio(-downSpeed);
            //Invoke(nameof(EffectReset), effectTime);
            StartCoroutine(EffectReset(pm));
        }
    }

    IEnumerator EffectReset(PlayerManager pm)
    {
        yield return new WaitForSeconds(effectTime[effectTime.Length - 1]);
        pm.playerController.SetMoveSpeedRatio(downSpeed);
        Destroy(gameObject);
    }
}
