using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [SerializeField] bool isTutorial;

    TutorialManager tutorial;
    RaceManager race;

    int openPlayer = -1;
    public bool isOpen = false;

    private void Awake()
    {
        //Žæ“¾‚Å‚«‚é•û‚ðŽæ“¾
        TryGetComponent(out tutorial);
        TryGetComponent(out race);
    }

    public void Pause(int playerNum)
    {
        openPlayer = playerNum;
        if (isTutorial)
        {
            tutorial.ViewPauseMenu(true);
        }
        else
        {
            race.ViewPauseMenu(true);
        }
        isOpen = true;
    }

    public void Close(int playerNum)
    {
        if(openPlayer == playerNum)
        {
            if (isTutorial)
            {
                tutorial.ViewPauseMenu(false);
            }
            else
            {
                race.ViewPauseMenu(false);
            }
        }
        openPlayer = -1;
        isOpen = false;
    }

    public void ReturnTitle(int playerNum)
    {
        if (openPlayer == playerNum)
        {
            if (isTutorial)
            {
                tutorial.ToTitleScene();
            }
            else
            {
                race.ToTitleScene();
            }
        }
    }
}
