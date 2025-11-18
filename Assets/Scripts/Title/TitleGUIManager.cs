using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using UnityEngine.UI;

public class TitleGUIManager : MonoBehaviour
{
    [SerializeField] TitleManager title;
    [SerializeField, Header("GUIs")] GameObject selectMenuPanel;
    [SerializeField] GameObject trapSelecterPanel;
    [SerializeField] Animation titlePanelAnim;
    [SerializeField] GameObject[] playerPanels = new GameObject[4];
    [SerializeField] GameObject[] charactorPanels = new GameObject[4];
    [SerializeField] GameObject[] trapPanels = new GameObject[4];
    [SerializeField] Image[] playingCharactorsImage = new Image[4];
    [SerializeField] Sprite[] charactorSprites = new Sprite[4];
    [SerializeField] Image[] charactorImage = new Image[4];
    [SerializeField] RectTransform[] trapIcons_store = new RectTransform[8];
    [SerializeField] List<List<RectTransform>> trapIcons_mine = new();
    [SerializeField] RectTransform[] selectCursors = new RectTransform[4];

    [SerializeField] Transform root;

    [ContextMenu("Debug_set")]
    public void SetIcons()
    {
        for (int i = 0; i < 4; i++)
        {
            trapIcons_mine.Add(new List<RectTransform>());
        }
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                trapIcons_mine[i].Add(root.GetChild(i).GetChild(1).GetChild(j).GetComponent<RectTransform>());
            }
        }
        print(trapIcons_mine.Count + ":" + trapIcons_mine[0].Count);
    }

    private void Awake()
    {
        SetIcons();

        //初期はタイトルパネル以外非表示
        for (int i = 0; i < 4; i++)
        {
            playerPanels[i].SetActive(false);
            charactorPanels[i].SetActive(true);
            trapPanels[i].SetActive(false);
            ViewCharactorImage(playerNum:i, isView:false);
            SetTrapSelectCursor(playerNum:i,trapId:0,isStore:true);
        }
        trapSelecterPanel.SetActive(false);
    }

    /// <summary>
    /// プレイヤーの人数に合わせて枠を用意
    /// </summary>
    /// <param name="playerCount"></param>
    public void SetPlayerPanel(int playerCount)
    {
        //人数を更新
        for (int i = 0; i < 4; i++)
        {
            if (i < playerCount) playerPanels?[i].SetActive(true);
            else playerPanels?[i].SetActive(false);
        }

        //もしタイトル画面ならキャラ選択画面へ移行
        if (title.nowPhase == TitleManager.NowPhase.Title) title.CharactorSelectView();
    }

    /// <summary>
    /// トラップ選択画面表示
    /// </summary>
    public void ViewTrapSelect()
    {
        trapSelecterPanel.SetActive(true);

        for (int i = 0; i < 4; i++)
        {
            charactorPanels[i].SetActive(false);
            trapPanels[i].SetActive(true);
        }
    }

    /// <summary>
    /// 準備ができたプレイヤーのキャラを表示
    /// </summary>
    /// <param name="playerNum"></param>
    /// <param name="isView"></param>
    public void ViewCharactorImage(int playerNum,bool isView)
    {
        charactorImage[playerNum].enabled = isView;
    }

    /// <summary>
    /// キャラ選択画面を表示
    /// </summary>
    public void ViewCharactorSelect()
    {
        //ピンぼけアニメーション再生
        titlePanelAnim.Play();
        //キャラ選択画面を表示
        selectMenuPanel.SetActive(true);
    }

    /// <summary>
    /// キャラ選択のプレイアブルキャラを変更
    /// </summary>
    /// <param name="playerNum"></param>
    /// <param name="charactorId"></param>
    public void ChangePlayingCharactorsImage(int playerNum,int charactorId)
    {
        playingCharactorsImage[playerNum].sprite = charactorSprites[charactorId];
    }

    /// <summary>
    /// タイトル画面を表示
    /// </summary>
    public void ViewTitle()
    {
        selectMenuPanel.SetActive(false);
        trapSelecterPanel.SetActive(false);
    }

    /// <summary>
    /// トラップのカーソルを指定
    /// </summary>
    /// <param name="playerNum"></param>
    /// <param name="trapId"></param>
    /// <param name="isStore"></param>
    public void SetTrapSelectCursor(int playerNum,int trapId,bool isStore)
    {
        if (isStore)
        {
            selectCursors[playerNum].position = trapIcons_store[trapId].position;
        }
        else
        {
            selectCursors[playerNum].position = trapIcons_mine[playerNum][trapId].position;
        }
    }
}
