using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.UI;

public class TitleGUIManager : MonoBehaviour
{
    [SerializeField] TitleManager title;
    [SerializeField] TrapStore trapStore;
    [SerializeField, Header("GUIs")] GameObject selectMenuPanel;
    [SerializeField] GameObject trapSelecterPanel;
    [SerializeField] Animation titlePanelAnim;
    [SerializeField] GameObject[] playerPanels = new GameObject[4];
    [SerializeField] GameObject[] charactorPanels = new GameObject[4];
    [SerializeField] GameObject[] trapPanels = new GameObject[4];
    [SerializeField] Image[] playingCharactorsImage = new Image[4];
    [SerializeField] Sprite[] playingCharactorSprites = new Sprite[4];
    [SerializeField] Image[] charactorImage = new Image[4];
    [SerializeField] Sprite[] charactorSprite = new Sprite[4];
    [SerializeField] RectTransform[] trapIcons_store = new RectTransform[8];
    [SerializeField] Image[] trapIconsImage_store = new Image[8];
    [SerializeField] List<List<RectTransform>> trapIcons_mine = new();
    [SerializeField] List<List<Image>> trapIconsImage_mine = new();
    [SerializeField] RectTransform[] selectCursors = new RectTransform[4];
    [SerializeField] HorizontalLayoutGroup SelectManulayoutGroup;
    [SerializeField] Transform root;

    /// <summary>
    /// //各プレイヤーの自分のトラップアイコンのRectTransformとImage取得
    /// </summary>
    public void SetTrapIcons_mine()
    {
        for (int i = 0; i < 4; i++)
        {
            trapIcons_mine.Add(new List<RectTransform>());
            trapIconsImage_mine.Add(new List<Image>());
        }
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                Transform sumpleTf = root.GetChild(i).GetChild(1).GetChild(j+1);
                trapIcons_mine[i].Add(sumpleTf.GetComponent<RectTransform>());
                trapIconsImage_mine[i].Add(sumpleTf.GetChild(0).GetComponent<Image>());
            }
        }
    }

    private void Awake()
    {
        //各プレイヤーの自分のトラップアイコンのRectTransformを取得
        SetTrapIcons_mine();

        //初期はタイトルパネル以外非表示
        for (int i = 0; i < 4; i++)
        {
            playerPanels[i].SetActive(false);
            charactorPanels[i].SetActive(true);
            trapPanels[i].SetActive(false);
            ViewCharactorImage(playerNum:i, isView:false);
            SetTrapSelectCursor(playerNum:i,trapId:0,isStore:false);
        }
        trapSelecterPanel.SetActive(false);

        //アイコンImageの適応
        for (int i = 0; i < trapStore.trapObjs.Count; i++) 
        {
            trapIconsImage_store[i].sprite = trapStore.trapObjs[i].GetComponent<TrapBase>().icon;
            trapIconsImage_store[i].enabled = true;
        }
        for (int i = trapStore.trapObjs.Count; i < 8; i++) 
        {
            trapIconsImage_store[i].enabled = false;
        }

        //カーソルを初期位置に移動
        for(int i = 0; i < 4; i++)
        {
            selectCursors[i].position = new Vector2(-10,-10);
        }

        //プレイヤーの自動レイアウトをオン
        SelectManulayoutGroup.enabled = true;
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
        //プレイヤーの自動レイアウトをオフ
        SelectManulayoutGroup.enabled = false;

        for (int i = 0; i < title.currentPlayerCount; i++)
        {
            charactorPanels[i].SetActive(false);
            trapPanels[i].SetActive(true);
            //カーソルの位置を変更
            SetTrapSelectCursor(playerNum: i, trapId: 0, isStore: false);
        }
    }

    /// <summary>
    /// 準備ができたプレイヤーのキャラを表示
    /// </summary>
    /// <param name="playerNum"></param>
    /// <param name="isView"></param>
    public void ViewCharactorImage(int playerNum,bool isView,int charaNum = -1)
    {
        if(charaNum >= 0)
        {
            charactorImage[playerNum].sprite = charactorSprite[charaNum];
        }
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
        playingCharactorsImage[playerNum].sprite = playingCharactorSprites[charactorId];
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

    /// <summary>
    /// プレイヤーの自分のトラップIconを選んだトラップに変更
    /// </summary>
    /// <param name="playernum"></param>
    /// <param name="trapId"></param>
    /// <param name="trapObjNum"></param>
    public void SetTrapIconSprite(int playernum,int trapId,int trapObjNum)
    {
        trapIconsImage_mine[playernum][trapObjNum].sprite = trapStore.trapObjs[trapId].GetComponent<TrapBase>().icon;
        trapIconsImage_mine[playernum][trapObjNum].enabled = true;
    }
}
