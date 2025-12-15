using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GoalSc : MonoBehaviour
{
    [SerializeField] RaceManager raceManager;
    [SerializeField] TextMeshProUGUI[] lapText;
    Dictionary<int, int> textDic = new();

    private void Start()
    {
        for (int i = 0; i < raceManager.playerDatas.Count; i++)
        {
            lapText[i].text = 0 + "/" + raceManager.lapCount;
            textDic.Add(raceManager.playerDatas[i].playerNum,i);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerManager pm = collision.transform.parent.GetComponent<PlayerManager>();
            
            //ƒS[ƒ‹
            if (pm.playerData.progress == 9)
            {
                if (pm.playerData.lapCount >= raceManager.lapCount)
                {
                    raceManager.PlayFinishAnimation();
                }
                
                if (pm.playerData.lapCount <= 0)
                {
                    lapText[pm.playerNum].text = 0 + "/" + raceManager.lapCount;
                }
                else
                {
                    lapText[textDic[pm.playerNum]].text = pm.playerData.lapCount + "/" + raceManager.lapCount;

                }
            }
        }
    }
}