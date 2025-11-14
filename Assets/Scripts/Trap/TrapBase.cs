using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapBase : MonoBehaviour
{
    public enum ETrapType
    {
        Up,     //バフ
        Down    //デバフ
    }

    [SerializeField, Header("罠の種類")] public ETrapType trapType;

    public int trapNum;

    protected Collider2D col;

    protected SpriteRenderer sr;

    public PlayerManager pm;

    [Header("トラップの効果時間")]
    public int[] effectTime = {1,2,4,8 };

    protected int rankingPower;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
    }
}
