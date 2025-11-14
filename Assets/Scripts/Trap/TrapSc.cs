using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TrapSc : TrapBase
{

    [SerializeField,Header("スピードの加算")] float speeds = 0.5f;

    public float objTime = 10;

    private void Start()
    {
        Invoke(nameof(TimeUp), objTime);
        trapNum = pm.playerNum;
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            pm = collision.transform.parent.GetComponent<PlayerManager>();
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
            
            //Invoke(nameof(EffectReset), effectTime);
            StartCoroutine(EffectReset(pm));
        }
    }

    IEnumerator EffectReset(PlayerManager pm)
    {
        rankingPower = pm.playerData.ranking;

        pm.playerController.SetMoveSpeedRatio(speeds);

        yield return new WaitForSeconds(effectTime[rankingPower]);

        pm.playerController.SetMoveSpeedRatio(-speeds);
    }

    //private void EffectReset()
    //{
    //    pm.playerController.SetMoveSpeedRatio(-speeds[rankingPower]);
    //}

    private void TimeUp()
    {
        col.enabled = false;
        sr.enabled = false;
        Destroy(gameObject, effectTime[effectTime.Length - 1]);
    }
}
