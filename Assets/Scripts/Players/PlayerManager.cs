using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public int playerNum;
    public PlayerData playerData;
    /// <summary>
    /// ゲームモード
    /// </summary>
    public enum GameMode
    {
        None,
        Race,
        Battle,
        Tutorial
    }
    public GameMode nowMode;

    public bool isDummy = false;

    [Header("他オブジェスクリプト")]
    public CorseCheck corseCheck;
    public PauseManager pause;
    public TrapStore trapStore;

    [Header("Playerスクリプト")]
    public PlayerController playerController;
    public PowerGage powerGage;
    public IconManager iconManager;
    public PlayerTrap trap;
}
