using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TrapSc : MonoBehaviour
{

    public int trapNum;

    private Collider2D col;

    private SpriteRenderer sr;

    private PlayerManager pm = null;

    [Header("トラップの効果時間")]
    public int effectTime;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            pm = collision.GetComponent<PlayerManager>();
            if (trapNum == pm.playerNum)
            {
                //バフ
                pm.playerController.SetMoveSpeedRatio(1.2f);
                //Debug.Log("良いですね");
            }
            else
            {
                //デバフ
                pm.playerController.SetMoveSpeedRatio(0.8f);
                //Debug.Log("消え失せろ");
            }
            col.enabled = false;
            sr.enabled = false;

            Invoke(nameof(EffectReset), effectTime);
        }
    }

    private void EffectReset()
    {
        pm.playerController.SetMoveSpeedRatio();
        //Debug.Log("おしめぇだ");
        if (trapNum != pm.playerNum)
        {
            Destroy(gameObject);
        }
    }
}
