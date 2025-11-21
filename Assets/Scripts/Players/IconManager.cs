using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class IconManager : MonoBehaviour
{
    [SerializeField] PlayerManager pm;
    [SerializeField, Tooltip("トラップ枠")] GameObject trapFrameObj;
    [SerializeField, Tooltip("トラップの使用禁止アイコン")] SpriteRenderer banIconSr;
    [SerializeField, Tooltip("トラップのアイコン")] SpriteRenderer trapIconSr;
    [SerializeField, Tooltip("置くトラップの種類のイメージ")] Sprite[] trapIconSp = new Sprite[4];
    
    [SerializeField] int bunCount;
    [SerializeField] float disappearanceTime;

    private void Start()
    {
        //持っているトラップのアイコンを取得
        for (int i = 0; i < 4; i++)
        {
            trapIconSp[i] = pm.playerController.trapObj[i].GetComponent<TrapBase>().icon;
        }
        //表示＆非表示 初期化
        trapFrameObj.SetActive(false);
        banIconSr.enabled = false;
        trapIconSr.enabled = true;

        //初期アイコンは1つ目のトラップに指定
        trapIconSr.sprite = trapIconSp[0];
    }

    /// <summary>
    /// トラップのSpriteを変更
    /// </summary>
    /// <param name="iconNum"></param>
    public void IconChange(int iconNum)
    {
        trapIconSr.sprite = trapIconSp[iconNum];
    }


    public void BanCheck(bool isBan)
    {
        trapFrameObj.SetActive(true);
        banIconSr.enabled = isBan;
        bunCount++;
        Invoke(nameof(IconReset), disappearanceTime);
    }

    private void IconReset()
    {
        if(bunCount == 1)
        {
            banIconSr.enabled = false;
            trapFrameObj.SetActive(false);
        }
        bunCount--;
    }
}
