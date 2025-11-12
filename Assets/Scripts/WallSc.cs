using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSc : MonoBehaviour
{

    [SerializeField,Header("耐久度")] float strengthMax;
    float strength;

    [SerializeField, Header("再構築のクールタイム")] float resetTime;

    private void Start()
    {
        strength = strengthMax;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            if(strength - collision.relativeVelocity.magnitude <= 0)
            {
                gameObject.SetActive(false);
                Invoke(nameof(WallReset), resetTime);
            }
            else
            {
                strength -= collision.relativeVelocity.magnitude;
            }
            print(collision.relativeVelocity.magnitude);
        }
    }

    public void WallReset()
    {
        strength = strengthMax;
        gameObject.SetActive(true);
    }
}
