using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    [SerializeField, Header("参加プレイヤーマネージャー")] JoinPlayerManager joinPlayerManager;
    [SerializeField, Header("GUIs")] GameObject selectMenuPanel;
    [SerializeField] GameObject trapSelecterPanel;
    [SerializeField] GameObject[] playerPanels = new GameObject[4];
    [SerializeField] Image[] charactorImages = new Image[4];
    [SerializeField] Sprite[] charactorSprites = new Sprite[4];
    [SerializeField] GameObject[] charactorReadyPanel = new GameObject[4];

    [SerializeField,Header("変数")] int playerCount;
    int[] playerCharactor = new int[4];
    bool[] playerIsLady = new bool[4];
    bool isTitle = true;
    bool isChara = false;
    bool isTrap = false;

    private void Awake()
    {
        isTitle = true;
        isChara = false;
        isTrap = false;
        for (int i = 0;i < 4; i ++)
        {
            playerPanels[i].SetActive(false);
            charactorReadyPanel[i].SetActive(false);
        }
    }

    bool CheckAllReady()
    {
        int count = 0;
        foreach(bool ready in playerIsLady)
        {
            if (ready) count++;
        }
        return count >= playerCount;
    }

    /// <summary>
    /// プレイヤー人数によって選択画面を表示、非表示に
    /// </summary>
    /// <param name="playerCount"></param>
    public void SetPlayerPanel(int playerCount)
    {
        //人数を更新
        this.playerCount = playerCount;
        for (int i = 0; i < 4; i++)
        {
            if(i < playerCount) playerPanels[i].SetActive(true);
            else playerPanels[i].SetActive(false);
        }

        //もしタイトル画面ならキャラ選択画面へ移行
        if (isTitle) CharactorSelectView();
    }

    /// <summary>
    /// タイトル画面から選択画面へと移動
    /// </summary>
    public void CharactorSelectView()
    {
        isTitle = false;
        isChara = true;
        selectMenuPanel.SetActive(true);
    }

    /// <summary>
    /// 選択画面画面からタイトルに戻る
    /// </summary>
    public void ReturnTitle()
    {
        isTitle = true;
        isChara = false;
        selectMenuPanel.SetActive(false);
        trapSelecterPanel.SetActive(false);
    }

    /// <summary>
    /// ゲームスタート
    /// </summary>
    public void StartGame()
    {
       joinPlayerManager.SetGameData();
        SceneManager.LoadScene("RaceScene");
    }

    /// <summary>
    /// ゲーム終了
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }

    #region プレイヤーからの入力

    /// <summary>
    /// 決定ボタン
    /// </summary>
    public void Decision(int playerId)
    {
        if (isTitle)
        {
            CharactorSelectView();
        }
        else if (isChara)
        {
            playerIsLady[playerId] = true;
            charactorReadyPanel[playerId].SetActive(true);
            joinPlayerManager.SetCharactorPlayerInfo(playerId, playerCharactor[playerId]);
            if (CheckAllReady())
            {
                StartGame();
            }
        }
    }

    /// <summary>
    /// キャンセルボタン
    /// </summary>
    public void Cancel(int playerId)
    {
        if (isChara)
        {
            playerIsLady[playerId] = false; 
            charactorReadyPanel[playerId].SetActive(false);
        }
    }

    /// <summary>
    /// キャラを変更
    /// </summary>
    /// <param name="playerId"></param>
    /// <param name="next"></param>
    public void ChangeCharactor(int playerId,bool next)
    {
        if (!isChara) return;
        if (playerIsLady[playerId]) return;
        playerCharactor[playerId] = (playerCharactor[playerId] + (next ? 1 : -1) + 4) % 4;
        charactorImages[playerId].sprite = charactorSprites[playerCharactor[playerId]];
    }

    #endregion
}
