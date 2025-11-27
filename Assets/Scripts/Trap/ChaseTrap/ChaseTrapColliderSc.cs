using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseTrapColliderSc : MonoBehaviour
{
    [SerializeField] ChaseTrapSc trap;
    [SerializeField] bool isSenser = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //対象が主なら無視
        if (collision.gameObject.CompareTag("Player"))
        {
            if(trap.pm == collision.transform.parent.GetComponent<PlayerManager>()) return;
        }

        if (isSenser)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                trap.ChasingPlayer(collision.transform);
            }
        }
        else
        {
            if (collision.CompareTag("Player"))
            {
                PlayerManager pm = collision.transform.parent.GetComponent<PlayerManager>();
                StartCoroutine(trap.GiveEffect(pm));
            }
            //プレイヤー以外にぶつかった
            else if(collision.CompareTag("MapObj"))
            {
                print(collision.name);
                trap.TimeUpAnim();
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //対象が主なら無視
        if (collision.gameObject.CompareTag("Player"))
        {
            if (trap.pm == collision.transform.parent.GetComponent<PlayerManager>()) return;
        }

        if (isSenser)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                trap.ChasingPlayer(collision.transform);
            }
        }
    }
}
