using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class GameData : MonoBehaviour
{
    [Header("プレイヤー人数")]
    public int playerCount;

    [Header("ランキング")]
    public int[] ranking = new int[4];
}
