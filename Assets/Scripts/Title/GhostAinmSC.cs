using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GhostAinmSC : MonoBehaviour
{
    [SerializeField] Image playingCharactorsImage;

    [SerializeField] TitleManager title;
    [SerializeField] int playerNum;
    public void PlayingCharaChengeFalse()
    {
        playingCharactorsImage.enabled = false;
    }

    public void PlayingCharaChengeTrue()
    {
        title.charaSelectFlag[playerNum] = false;
        playingCharactorsImage.enabled = true;
    }
}
