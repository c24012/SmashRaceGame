using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSc : MonoBehaviour
{

    [SerializeField] float strengthMax;
    float strength;

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
            }
            else
            {
                strength -= collision.relativeVelocity.magnitude;
            }
            
        }
    }

    public void WallReset()
    {
        strength = strengthMax;
        gameObject.SetActive(true);
    }
}
