using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Drawing;

public class IconManager : MonoBehaviour
{
    [SerializeField] PlayerManager pm;
    [SerializeField, Tooltip("トラップ枠")] GameObject trapFrameObj;
    [SerializeField, Tooltip("トラップの使用禁止アイコン")] SpriteRenderer banIconSr;
    [SerializeField, Tooltip("トラップのアイコン")] SpriteRenderer trapIconSr;
    [SerializeField, Tooltip("置くトラップの種類のイメージ")] Sprite[] trapIconSp = new Sprite[4];
    
    [SerializeField] int bunCount;
    [SerializeField] float disappearanceTime;
    [SerializeField] bool isView = false;

    private void Start()
    {
        //ダミーはスキップ
        if (pm.isDummy) return;

        GetIconSprite();
        //表示＆非表示 初期化
        trapFrameObj.SetActive(false);
        banIconSr.enabled = false;
        trapIconSr.enabled = true;

        //初期アイコンは1つ目のトラップに指定
        trapIconSr.sprite = trapIconSp[0];
    }

    /// <summary>
    /// トラッププレハブからSpriteを取得
    /// </summary>
    public void GetIconSprite()
    {
        trapIconSp = new Sprite[pm.playerController.trapObj.Length];
        //持っているトラップのアイコンを取得
        for (int i = 0; i < pm.playerController.trapObj.Length; i++)
        {
            trapIconSp[i] = pm.playerController.trapObj[i].GetComponent<TrapBase>().icon;
        }
    }

    /// <summary>
    /// トラップのSpriteを変更
    /// </summary>
    /// <param name="iconNum"></param>
    public void IconChange()
    {
        trapIconSr.sprite = trapIconSp[pm.trap.trapNum];
        //狐のお面確認
        CheckIllutionIcon();
    }


    public void BanCheck(bool isBan)
    {
        trapFrameObj.SetActive(true);
        banIconSr.enabled = isBan;
        bunCount++;
        Invoke(nameof(IconReset), disappearanceTime);
        IconChange();
    }

    private void IconReset()
    {
        if(bunCount == 1)
        {
            if (!isView)
            {
                banIconSr.enabled = false;
                trapFrameObj.SetActive(false);
            }
        }
        bunCount--;
    }

    public void ViewIcon(bool isView)
    {
        if (isView)
        {
            this.isView = true;
            trapFrameObj.SetActive(true);
        }
        else
        {
            this.isView = false;
            trapFrameObj.SetActive(false);
        }
    }

    public void CheckIllutionIcon()
    {
        Color32 color = trapIconSr.color;

        //幻影出現状態では狐のお面アイコンが半透明になる
        if (pm.playerController.isIllution)
        {
            if (pm.playerController.trapObj[pm.trap.trapNum] == pm.trapStore.trapObjs[9])
            {
                color.a = 128;
                trapIconSr.color = color;
                return;
            }
        }
        //条件外は戻す
        color.a = 255;
        trapIconSr.color = color;
    }
}
