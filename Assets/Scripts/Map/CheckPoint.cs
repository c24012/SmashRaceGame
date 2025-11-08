using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    [SerializeField] int setNum;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerData playerData = collision.GetComponent<PlayerManager>().playerData;
            playerData.progress = Mathf.Min(playerData.progress, setNum);
        }
    }
}
