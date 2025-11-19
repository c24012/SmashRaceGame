using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapBase : MonoBehaviour
{
    [Header("PlayerManager")]
    public PlayerManager pm = null;
    [Header("Gui関連"),Tooltip("表示アイコン")]
    public Sprite icon = null;
    [Tooltip("トラップの説明")]
    public string info = null;
    [Header("即発動トラップか")]
    public bool isInstantActive = false;
    [Header("トラップの効果時間")]
    public float[] effectTime = { 1, 2, 4, 8 };

    protected int rankingPower; //設置時の主の順位
    protected Collider2D col;   //トラップの当たり判定
    protected SpriteRenderer sr;//トラップの見た目

    private void Awake()
    {
        if (isInstantActive) return;
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
    }
}
