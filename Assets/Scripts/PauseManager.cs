using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    int openPlayer = -1;
    public bool isOpen = false;

    public void Pause(int playerNum)
    {
        openPlayer = playerNum;
        gameObject.SendMessage("SetPausePlayerText", playerNum);
        gameObject.SendMessage("ViewPauseMenu", true);
        isOpen = true;
    }

    public void Close(int playerNum)
    {
        if(openPlayer == playerNum)
        {
            gameObject.SendMessage("ViewPauseMenu", false);
        }
        openPlayer = -1;
        isOpen = false;
    }

    public void ReturnTitle(int playerNum)
    {
        if (openPlayer == playerNum)
        {
            gameObject.SendMessage("ToTitleScene");
        }
    }
}
