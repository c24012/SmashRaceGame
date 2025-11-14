using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GoalSc : MonoBehaviour
{
    [SerializeField] RaceManager raceManager;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerData playerData = collision.transform.parent.GetComponent<PlayerManager>().playerData;
            //ÉSÅ[Éã
            if (playerData.progress == 9)
            {
                if (playerData.lapCount >= raceManager.lapCount)
                {
                    print($"{playerData.playerNum}PÇ™ÉSÅ[ÉãÅI");
                    raceManager.FinishRace();
                }
            }
        }
    }
}