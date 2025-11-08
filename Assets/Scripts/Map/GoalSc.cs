using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GoalSc : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerData playerData = collision.GetComponent<PlayerManager>().playerData;
            if (playerData.progress == 9)
            {
                if(playerData.lapCount >= 3)
                {
                    print($"{playerData.playerNum}P‚ªƒS[ƒ‹I");
                }
                else
                {
                    playerData.lapCount++;
                    print($"{playerData.playerNum}P {playerData.lapCount}ü–Ú");
                }
                playerData.progress = 0;
            }
        }
    }
}
