using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GhostAinmSC : MonoBehaviour
{
    [SerializeField] GameObject inImage;

    [SerializeField] TitleManager title;
    [SerializeField] int playerNum;

    public void PlayingChengeFalse()
    {
        if (title.nowPhase == TitleManager.NowPhase.CountSelect)
        {
            title.countChangingFlag = true;
        }
        inImage.SetActive(false);
    }

    public void PlayingChengeTrue()
    {
        if(title.nowPhase == TitleManager.NowPhase.CountSelect)
        {
            title.countChangingFlag = false;
        }
        else if(title.nowPhase == TitleManager.NowPhase.CharaSelect)
        {
            title.charaChangingFlag[playerNum] = false;
        }
        inImage.SetActive(true);
    }
}
