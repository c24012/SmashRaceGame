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
            PlayerManager pm = collision.transform.parent.GetComponent<PlayerManager>();
            if (pm.isDummy) return;
            pm.playerData.progress = Mathf.Min(pm.playerData.progress, setNum);
        }
    }
}
