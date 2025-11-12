using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    [SerializeField] int playerCount;
    [SerializeField] Text playerNumText;
    [SerializeField] GameData gameData;

    public void AddPlayer()
    {
        playerCount = Mathf.Min(++playerCount, 4);
        playerNumText.text = playerCount.ToString();
    }
    public void RemovePlayer()
    {
        playerCount = Mathf.Max(--playerCount, 2);
        playerNumText.text = playerCount.ToString();
    }

    void SetGameData()
    {
        gameData.playerCount = playerCount;
    }

    /// <summary>
    /// ゲームスタート
    /// </summary>
    public void StartGame()
    {
        SetGameData();
        SceneManager.LoadScene("RaceScene");
    }

    /// <summary>
    /// ゲーム終了
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }
}
