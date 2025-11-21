using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultManager : MonoBehaviour
{
    public Image[] charaImage;

    public Sprite[] charaSp;

    public GameData data;

    public PlayableDirector fadeOutAnim;
    private void Start()
    {

        for(int i = 3; i > data.playerCount - 1; i--)
        {
            charaImage[i].enabled = false;
        }

       for(int i = 0;i < data.playerCount; i++)
        {
            charaImage[i].sprite = charaSp[data.ranking[i] + (i * 4)];
        }

        fadeOutAnim.Play();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("TitleScene");
        }
    }
}
