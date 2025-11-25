using System;
using UnityEngine;

public class TrapBase : MonoBehaviour
{
    [NonSerialized]
    public PlayerManager pm = null;
    [Header("Gui関連"),Tooltip("表示アイコン")]
    public Sprite icon = null;
    [Tooltip("トラップの説明")]
    public string info = null;
    [Header("トラップの効果時間")]
    public float[] effectTime = { 1, 2, 4, 8 };
    [Header("即発動トラップか")]
    public bool isInstantActive = false;

    protected int rankingPower;     //設置時の主の順位
    protected GameObject trapObj;   //設置するトラップ本体

    private void Awake()
    {
        if (isInstantActive) return;
        trapObj = transform.GetChild(0).gameObject;
    }

    private void Start()
    {
        //PlayerManagerを取得
        rankingPower = pm.playerData.ranking;
    }
}
