using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class IconManager : MonoBehaviour
{
    [SerializeField, Tooltip("トラップのデフォルト表示のオブジェ")] GameObject trapImageObj;
    [SerializeField, Tooltip("トラップの禁止のアイコンのオブジェ")] GameObject banIconObj;
    [SerializeField, Tooltip("トラップのアイコンのオブジェ")] GameObject trapIconObj;
    [SerializeField, Tooltip("置くトラップの種類のイメージ")] Sprite[] trapIconSp;
    SpriteRenderer trapIconSr;
    //private bool isDisappearance;
    [SerializeField] int buttonCount;
    [SerializeField] float DisappearanceTime;

    private void Start()
    {
        trapImageObj.SetActive(false);
        banIconObj.SetActive(false);
        trapIconSr = trapIconObj.GetComponent<SpriteRenderer>();
        trapIconSr.sprite = trapIconSp[0];
    }

    public void IconChange(int iconNum)
    {
        trapIconSr.sprite = trapIconSp[iconNum];
    }

    public void BanCheck(bool isBan)
    {
        trapImageObj.SetActive(true);
        banIconObj.SetActive(isBan);
        //Invoke(new Action(() => { trapImageObj.SetActive(false); banIconObj.SetActive(false); }).Method.Name, 3);
        //StartCoroutine(IconReset());
        buttonCount += 1;
        Invoke(nameof(IconReset), DisappearanceTime);
    }

    private void IconReset()
    {
        if(buttonCount == 1)
        {
            banIconObj.SetActive(false);
            trapImageObj.SetActive(false);
        }
        buttonCount -= 1;

    }
}
