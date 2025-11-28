using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class TitleGUIManager : MonoBehaviour
{
    [SerializeField] TitleManager title;
    [SerializeField] TrapStore trapStore;

    [SerializeField, Header("人数選択画面")] GameObject countSelectPanel;
    [SerializeField] Animator countSelect_GhostAnim;
    [SerializeField] Text countSlect_PlayerCountText;

    [SerializeField, Header("キャラ選択画面")] GameObject[] playerPanels = new GameObject[4];
    [SerializeField] GameObject[] joinPanels = new GameObject[4];
    [SerializeField] GameObject[] CharaSignBoardPanels = new GameObject[4];
    [SerializeField] Animator[] GhostAnim;

    [SerializeField, Header("アイテム選択画面")] GameObject trapSelecterPanel;
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
    [SerializeField] Animator lastCheckSignBoardAnim;
    [SerializeField] Image[] iconImage;
    [SerializeField] Sprite[] iconSp;
    [SerializeField] GameObject infoHintUiObj;
    [SerializeField] GameObject cursorsObj;

    //画面遷移
    [SerializeField, Header("画面遷移アニメーション")] PlayableDirector countAnim_In;
    [SerializeField] PlayableDirector R_CountAnim_In;
    [SerializeField] PlayableDirector countAnim_Out;
    [SerializeField] PlayableDirector R_CountAnim_Out;
    [SerializeField] PlayableDirector R_CharaAnim_In;
    [SerializeField] PlayableDirector charaSelectOutAnim;
    [SerializeField] PlayableDirector trapSelectInAnim;
    [SerializeField] PlayableDirector fadeIn;


    int playerNum_local;
    int charactorId_local;

    /// <summary>
    /// //各プレイヤーの自分のトラップアイコンのRectTransformとImage取得
    /// </summary>
    void SetTrapIcons_mine()
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
                Transform sumpleTf = root.GetChild(i).GetChild(2).GetChild(j + 1);
                trapIcons_mine[i].Add(sumpleTf.GetComponent<RectTransform>());
                trapIconsImage_mine[i].Add(sumpleTf.GetComponent<Image>());
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
            CharaSignBoardPanels[i].SetActive(true);
            trapPanels[i].SetActive(false);
            ViewCharactorImage(playerNum: i, isView: false);
            SetTrapSelectCursor(playerNum: i, trapId: 0, isStore: false);
        }
        SetPhasePanel(TitleManager.NowPhase.Title);

        //アイコンImageの適応
        for (int i = 0; i < trapStore.trapObjs.Count; i++)
        {
            trapIconsImage_store[i].sprite = trapStore.trapObjs[i].GetComponent<TrapBase>().icon;
            //trapIconsImage_store[i].enabled = true;
        }
        for (int i = trapStore.trapObjs.Count; i < 8; i++)
        {
            trapIconsImage_store[i].enabled = false;
        }

        //カーソルを初期位置に移動
        for (int i = 0; i < 4; i++)
        {
            selectCursors[i].position = new Vector2(-10, -10);
        }

        //プレイヤーの自動レイアウトをオン
        SelectManulayoutGroup.enabled = true;
    }

    #region #タイトル画面

    /// <summary>
    /// タイトル画面を表示
    /// </summary>
    public void ViewTitle()
    {
        SetPhasePanel(TitleManager.NowPhase.Title);
    }

    #endregion

    #region #人数選択画面

    /// <summary>
    /// 人数選択画面を表示
    /// </summary>
    public void ViewCountSelect()
    {
        //アニメーション起動
        countAnim_In.Play();
        //アニメーション中フラグオン
        title.isPlayingAnim = true;
        countAnim_In.stopped += StopedAnimation;
    }

    /// <summary>
    /// タイトルシーンに戻る
    /// </summary>
    public void ReturnTitle()
    {
        //アニメーション起動
        R_CountAnim_In.Play();
        //アニメーション中フラグオン
        title.isPlayingAnim = true;
        R_CountAnim_In.stopped += StopedAnimation;
    }
    #endregion

    #region #キャラ選択画面

    /// <summary>
    /// キャラ選択画面を表示
    /// </summary>
    public void ViewCharactorSelect()
    {
        //幽霊上昇アニメーション
        countAnim_Out.Play();
        //アニメーション中フラグオン
        title.isPlayingAnim = true;
        countAnim_Out.stopped += StopedAnimation;
    }

    /// <summary>
    /// 人数選択画面に戻る
    /// </summary>
    public void ReturnCountSelect()
    {
        //退散アニメーション起動
        R_CharaAnim_In.Play();
        //アニメーション中フラグオン
        title.isPlayingAnim = true;
        R_CharaAnim_In.stopped += ReverseCharaSelectInAnim_stopped;
    }
    private void ReverseCharaSelectInAnim_stopped(PlayableDirector obj)
    {
        //人数選択画面に戻る
        R_CountAnim_Out.Play();
        R_CountAnim_Out.stopped += StopedAnimation;
    }


    #endregion

    /// <summary>
    /// フェーズによって必要パネル表示を切り替え
    /// </summary>
    /// <param name="now"></param>
    void SetPhasePanel(TitleManager.NowPhase now)
    {
        switch (now)
        {
            //タイトル
            case TitleManager.NowPhase.Title:
                countSelectPanel.SetActive(false);
                for (int i = 0; i < 4; i++)
                {
                    joinPanels[i].SetActive(false);
                    CharaSignBoardPanels[i].SetActive(false);
                }
                trapSelecterPanel.SetActive(false);
                break;
            //人数選択
            case TitleManager.NowPhase.CountSelect:
                countSelectPanel.SetActive(true);
                for (int i = 0; i < 4; i++)
                {
                    joinPanels[i].SetActive(false);
                    CharaSignBoardPanels[i].SetActive(false);
                }
                trapSelecterPanel.SetActive(false);
                break;
            //キャラ選択
            case TitleManager.NowPhase.CharaSelect:
                countSelectPanel.SetActive(false);
                trapSelecterPanel.SetActive(false);
                break;
            //アイテム選択
            case TitleManager.NowPhase.TrapSelect:
                countSelectPanel.SetActive(false);
                for (int i = 0; i < 4; i++)
                {
                    joinPanels[i].SetActive(false);
                    CharaSignBoardPanels[i].SetActive(false);
                    trapPanels[i].SetActive(true);
                    //カーソルの位置を変更
                    if (i < title.currentPlayerCount) SetTrapSelectCursor(playerNum: i, trapId: 0, isStore: false);
                }
                trapSelecterPanel.SetActive(true);
                break;
        }
    }

    /// <summary>
    /// アニメーション終了時にフラグを解除
    /// </summary>
    /// <param name="anim"></param>
    void StopedAnimation(PlayableDirector anim)
    {
        anim.stopped -= StopedAnimation;
        //アニメーション中フラグ解除
        title.isPlayingAnim = false;
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
            if (i < playerCount)
            {
                if (playerPanels[i] != null) playerPanels[i].SetActive(true);
            }
            else
            {
                if (playerPanels[i] != null) playerPanels[i].SetActive(false);
            }
        }
    }

    /// <summary>
    /// 参加した人を表示していく
    /// </summary>
    /// <param name="playerCount"></param>
    public void SetJoinPlayer(int playerCount)
    {
        //人数を更新
        for (int i = 0; i < 4; i++)
        {
            if (i < playerCount)
            {
                joinPanels[i].SetActive(false);
                CharaSignBoardPanels[i].SetActive(true);
            }
            else
            {
                joinPanels[i].SetActive(true);
                CharaSignBoardPanels[i].SetActive(false);
            }
        }
    }

    /// <summary>
    /// トラップ選択画面表示
    /// </summary>
    public void ViewTrapSelect()
    {
        //プレイヤーの自動レイアウトをオフ
        SelectManulayoutGroup.enabled = false;

        //アニメーション開始
        title.isPlayingAnim = true;
        charaSelectOutAnim.stopped += CharaSelectOutAnim_stopped;
        charaSelectOutAnim.Play();
    }
    void CharaSelectOutAnim_stopped(PlayableDirector obj)
    {
        //登録を削除
        charaSelectOutAnim.stopped -= CharaSelectOutAnim_stopped;
        //アイテム選択画面を表示
        SetPhasePanel(TitleManager.NowPhase.TrapSelect);

        //アニメーション開始
        trapSelectInAnim.stopped += TrapSelectInAnim_stopped;
        trapSelectInAnim.Play();
    }
    void TrapSelectInAnim_stopped(PlayableDirector obj)
    {
        //登録を解除
        trapSelectInAnim.stopped -= TrapSelectInAnim_stopped;
        //アニメーション終了
        title.isPlayingAnim = false;
        //キャラによってアイコンを変更
        for (int i = 0; i < title.currentPlayerCount; i++)
        {
            iconImage[i].sprite = iconSp[title.playerInfoList[i].charactorNum];
        }
        //変更後にカーソルを表示
        cursorsObj.SetActive(true);
        // アニメーション終了に合わせてトラップアイコンを表示
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                trapIconsImage_mine[i][j].enabled = true;
            }
        }
        for (int i = 0; i < 8; i++)
        {
            trapIconsImage_store[i].enabled = true;
        }
        //説明のヒントUIも表示
        infoHintUiObj.SetActive(true);
    }

    /// <summary>
    /// 準備ができたプレイヤーのキャラを表示
    /// </summary>
    /// <param name="playerNum"></param>
    /// <param name="isView"></param>
    public void ViewCharactorImage(int playerNum, bool isView, int charaNum = -1)
    {
        if (charaNum >= 0)
        {
            charactorImage[playerNum].sprite = charactorSprite[charaNum];
        }
        charactorImage[playerNum].enabled = isView;
    }


    /// <summary>
    /// 人数選択の看板の数字変更&アニメーション
    /// </summary>
    /// <param name="inputValue"></param>
    /// <param name="count"></param>
    public void ChangePlayerCountText(int inputValue, int count)
    {
        //入力がプラスの場合は左
        if (inputValue > 0) countSelect_GhostAnim.SetTrigger("Left");
        //マイナスの場合は右回転
        else countSelect_GhostAnim.SetTrigger("Right");
        //数字を表示
        countSlect_PlayerCountText.text = count.ToString();
    }

    /// <summary>
    /// キャラ選択のプレイアブルキャラを変更
    /// </summary>
    /// <param name="playerNum"></param>
    /// <param name="charactorId"></param>
    public void ChangePlayingCharactorsImage(int playerNum, int charactorId)
    {
        playerNum_local = playerNum;
        playingCharactorsImage[playerNum_local].sprite = playingCharactorSprites[charactorId];
        if (charactorId - 1 == charactorId_local || charactorId_local == charactorId + 3)
        {
            GhostAnim[playerNum].SetTrigger("Right");
        }
        else
        {
            GhostAnim[playerNum].SetTrigger("Left");
        }
        charactorId_local = charactorId;
    }


    /// <summary>
    /// トラップのカーソルを指定
    /// </summary>
    /// <param name="playerNum"></param>
    /// <param name="trapId"></param>
    /// <param name="isStore"></param>
    public void SetTrapSelectCursor(int playerNum, int trapId, bool isStore)
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
    public void SetTrapIconSprite(int playernum, int trapId, int trapObjNum)
    {
        trapIconsImage_mine[playernum][trapObjNum].sprite = trapStore.trapObjs[trapId].GetComponent<TrapBase>().icon;
        trapIconsImage_mine[playernum][trapObjNum].enabled = true;
    }

    /// <summary>
    /// 最終準備確認お化けのアニメーション
    /// </summary>
    /// <param name="isView"></param>
    public void PlayLastCheckSignBoardAnim(bool isView)
    {
        //今の同じ状態は返却
        if (lastCheckSignBoardAnim.GetBool("isDown") == isView) return;
        lastCheckSignBoardAnim.SetBool("isDown", isView);
    }

    /// <summary>
    /// シーン移動のフェードイン再生
    /// </summary>
    public void PlayFadeIn()
    {
        fadeIn.Play();
    }
}
