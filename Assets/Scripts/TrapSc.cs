using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TrapSc : TrapBase
{

    private float[] speeds = {0.1f,0.5f,1,2 };

    public float objTime = 10;

    private void Start()
    {
        Invoke(nameof(TimeUp), objTime);
        trapNum = pm.playerNum;
        rankingPower = pm.playerData.ranking;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            pm = collision.GetComponent<PlayerManager>();
            //if (trapNum == pm.playerNum)
            //{
            //    //バフ
            //    pm.playerController.SetMoveSpeedRatio(1.2f);
            //    //Debug.Log("良いですね");
            //}
            //else
            //{
            //    //デバフ
            //    pm.playerController.SetMoveSpeedRatio(0.8f);
            //    //Debug.Log("消え失せろ");
            //    col.enabled = false;
            //    sr.enabled = false;
            //}
            pm.playerController.SetMoveSpeedRatio(speeds[rankingPower]);
            Invoke(nameof(EffectReset), effectTime);
        }
    }

    private void EffectReset()
    {
        pm.playerController.SetMoveSpeedRatio(-speeds[rankingPower]);
    }

    private void TimeUp()
    {
        col.enabled = false;
        sr.enabled = false;
        Destroy(gameObject, effectTime);
    }
}
