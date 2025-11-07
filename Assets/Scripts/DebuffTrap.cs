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
            pm = collision.GetComponent<PlayerManager>();
            col.enabled = false;
            sr.enabled = false;
            pm.playerController.SetMoveSpeedRatio(downSpeed);
            Invoke(nameof(EffectReset), effectTime);
        }
    }

    private void EffectReset()
    {
        pm.playerController.SetMoveSpeedRatio(-downSpeed);
        Destroy(gameObject);
    }
}
