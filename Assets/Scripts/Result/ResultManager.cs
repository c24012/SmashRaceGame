using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultManager : MonoBehaviour
{
    public Image[] charaImage;

    public Sprite[] charaSp;

    public GameData data;

    private void Start()
    {

        for(int i = 3; i > data.playerCount - 1; i--)
        {
            charaImage[i].enabled = false;
        }

       for(int i = 0;i < data.playerCount; i++)
        {
            charaImage[i].sprite = charaSp[data.ranking[i] + (i * 4)];
        }
    }
}
