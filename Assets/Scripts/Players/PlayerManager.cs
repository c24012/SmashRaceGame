using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public int playerNum;
    public PlayerData playerData;

    [Header("他オブジェスクリプト")]
    public CorseCheck corseCheck;
    public PauseManager pause;

    [Header("Playerスクリプト")]
    public PlayerController playerController;
    public PowerGage powerGage;
    public IconManager iconManager;
    public PlayerTrap trap;
}
