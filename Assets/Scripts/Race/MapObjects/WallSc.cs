using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSc : MonoBehaviour
{

    [SerializeField,Header("耐久度")] float strengthMax;
    float strength;

    [SerializeField, Header("再構築のクールタイム")] float resetTime;

    [SerializeField, Header("アニメーターコントローラー")] Animator anim;

    [SerializeField, Header("スプライトレンダラー")] SpriteRenderer sr;

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
                anim.SetBool("IsFinish", true);
                
                Invoke(nameof(WallReset), resetTime);
            }
            else
            {
                anim.SetTrigger("Damage");
                strength -= collision.relativeVelocity.magnitude;
            }
        }
    }

    private void WallReset()
    {
        anim.SetBool("IsFinish", false);
    }

    public void LayerBack()
    {
        sr.sortingLayerName = "BackGround";
    }

    public void LayerFoward()
    {
        strength = strengthMax;
        sr.sortingLayerName = "FowardObj";
    }
}
