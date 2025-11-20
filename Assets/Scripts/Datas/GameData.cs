using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    [Header("プレイヤー人数")]
    public int playerCount;

    [Header("プレイヤー選択情報")]
    public List<PlayerInfo> playerInfoList = new();

    [Header("ランキング")]
    public int[] ranking = new int[4];
}
