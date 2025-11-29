using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[Serializable]
/// <summary>
/// プレイヤーの詳細データ
/// </summary>
public class PlayerInfo
{
    public PlayerInput playerInput;     //PlayerInputコンポーネント
    public int playerIndex;             //PlayerInputの登録番号
    public InputDevice device;          //使っているデバイス
    public int charactorNum = 0;        //プレイヤーの選んだキャラ
    public int[] trapNum = new int[4];  //プレイヤーの選んだ罠

    public PlayerInfo(int playerIndex, PlayerInput playerInput, InputDevice device)
    {
        this.playerInput = playerInput;
        this.playerIndex = playerIndex;
        this.device = device;
    }

    public void SetPlayerIndex()
    {
        this.playerIndex = this.playerInput.playerIndex;
    }

    public void SetCharactor(int selectCharactor)
    {
        charactorNum = selectCharactor;
    }

    public void SetTrap(int[,] selectTraps)
    {
        for(int i = 0; i < 4; i++)
        {
            trapNum[i] = selectTraps[this.playerIndex,i];
        }
    }

    public void RemovePlayerInput()
    {
        playerInput = null;
    }
}

public class TitleManager : MonoBehaviour
{
    [SerializeField, Header("参加プレイヤーマネージャー")] JoinPlayerManager join_m;
    [SerializeField, Header("GUIマネージャー")] TitleGUIManager gui_m;
    [SerializeField, Header("ゲームデータ")] GameData gameData;

    public List<PlayerInfo> playerInfoList = new(); //プレイヤーの情報登録リスト
    [Header("変数")] public int currentPlayerCount; //プレイヤーの数
    [SerializeField] int trapWidthCount = 4;
    [SerializeField] int trapHighCount = 2;

    int maxPlayerCount = 1;                         //決められたプレイヤー人数
    int[] playerCharactor = { 0, 1, 2, 3 };         //各プレイヤーのキャラクターId
    int[,] playerSelectedTraps =                    //各プレイヤーのトラップId
        {{-1,-1,-1,-1},{-1,-1,-1,-1},{-1,-1,-1,-1},{-1,-1,-1,-1}};
    int[] playerSelectingTrap_store = new int[4];   //トラップ選択画面のカーソルの場所(トラップ一覧)
    int[] playerSelectingTrap_mine = new int[4];    //トラップ選択画面のカーソルの場所(自分のトラップ)
    bool[] isStoreTrapTable = new bool[4];          //自分のテーブルを選択中か一覧のテーブルを選択中か
    bool[] playerIsLady = new bool[4];              //プレイヤーの準備状態
    bool isLadyGame = false;                        //ゲーム開始できるか
    bool isStartGame = false;                       //ゲーム開始

    public bool countChangingFlag = false;
    public bool[] charaChangingFlag = new bool[4];  //キャラ選択のアニメーション中か
    public bool isPlayingAnim = false;              //アニメーション再生中か

    bool isTutorial = false;

    public enum NowPhase
    {
        Title,
        CountSelect,
        CharaSelect,
        TrapSelect,
    }
    public NowPhase nowPhase = NowPhase.Title;      //現在のフェーズ

    /// <summary>
    /// 全員が準備できたかの確認
    /// </summary>
    /// <returns>できたかどうか</returns>
    bool CheckAllReady()
    {
        int count = 0;
        //準備出来た人数をカウント
        foreach (bool ready in playerIsLady) if (ready) count++;
        //今いるプレイヤー全員と一致するかを返却
        return count >= maxPlayerCount;
    }

    /// <summary>
    /// 全員がアイテムを選択したかの確認
    /// </summary>
    /// <returns></returns>
    bool CheckAllSelected()
    {
        //アイテムが全部選択されたか
        for(int i = 0; i < maxPlayerCount; i++)
        {
            for(int j = 0; j < 4; j++)
            {
                if (playerSelectedTraps[i, j] == -1) return false;
            }
        }

        int count = 0;
        //準備出来た人数をカウント
        for(int i = 0;i < maxPlayerCount; i++)
        {
            if (!isStoreTrapTable[i]) count++;
        }
        //今いるプレイヤー全員と一致するかを返却
        return count > maxPlayerCount;
    }

    /// <summary>
    /// 人数選択画面へ
    /// </summary>
    public void CountSelectView(bool isReturn = false)
    {
        if (isReturn)
        {
            //タイトルに戻る
            gui_m.ReturnTitle();
            nowPhase = NowPhase.Title;
            //全員の準備状態を解除
            for (int i = 0; i < currentPlayerCount; i++) 
            {
                //準備OK状態にする
                playerIsLady[i] = false;
                gui_m.ViewCharactorImage(i, false);
            }
        }
        else
        {
            //プレイヤー人数選択画面
            gui_m.ViewCountSelect();
            nowPhase = NowPhase.CountSelect;
        }
    }

    /// <summary>
    /// トラップ選択画面へ
    /// </summary>
    public void TrapSelectView(bool isReturn = false)
    {
        if (isReturn)
        {

        }
        else
        {
            //トラップ選択画面を表示
            gui_m.ViewTrapSelect();
            nowPhase = NowPhase.TrapSelect;
        }
    }

    /// <summary>
    /// キャラ選択画面へ
    /// </summary>
    public void CharactorSelectView(bool isReturn = false)
    {
        if (isReturn)
        {
            //人数選択画面に戻る
            gui_m.ReturnCountSelect();
            nowPhase = NowPhase.CountSelect;
        }
        else
        {
            //キャラ選択画面を表示
            gui_m.ViewCharactorSelect();
            nowPhase = NowPhase.CharaSelect;
        }
    }

    /// <summary>
    /// タイトル画面へ
    /// </summary>
    public void TitleView(bool isReturn = false)
    {
        nowPhase = NowPhase.Title;
        if (isReturn)
        {

        }
        else
        {
            //タイトル画面を表示
            gui_m.ViewTitle();
        }
    }

    /// <summary>
    /// PlayerInfoのPlayerIndex更新
    /// </summary>
    public void PlayerInfoDataUpdate()
    {
        if(nowPhase == NowPhase.TrapSelect)
        {
            SceneManager.LoadScene("TitleScene");
        }

        for(int i = 0;i< playerInfoList.Count; i++)
        {
            playerInfoList[i].SetPlayerIndex();
        }
        for (int i = 0; i < 4; i++) 
        {
            //準備OK状態を解除する
            playerIsLady[i] = false;
            gui_m.ViewCharactorImage(i, false);
        }
    }

    /// <summary>
    /// プレイヤーのキャラクターを保存
    /// </summary>
    /// <param name="playerIndex"></param>
    /// <param name="charaNum"></param>
    void SetCharactorPlayerInfo(int playerIndex, int charaNum)
    {
        playerInfoList.Find((x) => x.playerIndex == playerIndex).SetCharactor(charaNum);
    }

    /// <summary>
    /// プレイヤーのトラップを保存
    /// </summary>
    /// <param name="playerIndex"></param>
    /// <param name="charaNum"></param>
    void SetTrapsPlayerInfo(int playerIndex, int[,] trapsNum)
    {
        playerInfoList.Find((x) => x.playerIndex == playerIndex).SetTrap(trapsNum);
    }

    /// <summary>
    /// 持っているプレイヤー詳細データを共有用データPrefabに保存
    /// </summary>
    void SetGameData()
    {
        //人数
        gameData.playerCount = currentPlayerCount;

        //PlayerInputを忘れる
        for (int i = 0; i < playerInfoList.Count; i++)
        {
            playerInfoList[i].RemovePlayerInput();
        }
        //各プレイヤーの詳細データ
        gameData.playerInfoList = playerInfoList;
    }

    /// <summary>
    /// ゲームスタート
    /// </summary>
    public void StartGame()
    {
        //プレイヤーの情報を共有用データに保存
        SetGameData();

        //画面の暗転アニメーションが入る
        gui_m.PlayFadeIn();
    }

    /// <summary>
    /// フェードインが終了
    /// </summary>
    public void FinishTitleFadeIn()
    {
        //ボタン検知をオフ
        join_m.DisableActions();

        if (isTutorial)
        {
            //レースシーンへ
            SceneManager.LoadScene("TutorialScene");
            return;
        }
        //レースシーンへ
        SceneManager.LoadScene("RaceScene");
    }

    /// <summary>
    /// ゲーム終了
    /// </summary>
    public void QuitGame()
    {
        //タイトル以外は返却
        if (nowPhase != NowPhase.Title) return;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
#else
        Application.Quit();//ゲームプレイ終了
#endif
    }

    /// <summary>
    /// チュートリアル開始
    /// </summary>
    public void StartTutorial()
    {
        //タイトル以外は返却
        if (nowPhase != NowPhase.Title) return;

        //チュートリアルフラグをオン
        isTutorial = true;

        //画面の暗転アニメーションが入る
        gui_m.PlayFadeIn();
    }

    #region #プレイヤーからの入力

    /// <summary>
    /// 決定ボタン
    /// </summary>
    public void Decision(int playerId)
    {
        //ゲーム開始後は受け付けない
        if (isStartGame) return;

        //何かしらの遷移アニメーション中は受け付けない
        if (isPlayingAnim) return;

        //タイトル
        if (nowPhase == NowPhase.Title)
        {
            CountSelectView();
        }
        //人数選択画面
        else if (nowPhase == NowPhase.CountSelect)
        {
            //人数分キャラパネルを用意
            gui_m.SetPlayerPanel(maxPlayerCount);
            CharactorSelectView();
        }
        //キャラ選択画面
        else if (nowPhase == NowPhase.CharaSelect)
        {
            //準備OK状態にする
            playerIsLady[playerId] = true;
            gui_m.ViewCharactorImage(playerId, true, playerCharactor[playerId]);
            //プレイヤーのキャラをデータ型に代入
            SetCharactorPlayerInfo(playerId, playerCharactor[playerId]);
            //他のプレイヤーも準備ができたらトラップ選択画へ
            if (CheckAllReady())
            {
                //トラップ選択画面へ
                TrapSelectView();
                //準備フラグを全員リセット
                for (int i = 0; i < 4; i++)
                {
                    playerIsLady[i] = false;
                }
            }
        }
        //トラップ画面
        else if (nowPhase == NowPhase.TrapSelect)
        {
            //一覧選択中ならトラップ決定
            if (isStoreTrapTable[playerId])
            {
                //選んでいる自分のトラップ枠に今選んだトラップIdを指定
                playerSelectedTraps[playerId, playerSelectingTrap_mine[playerId]] = playerSelectingTrap_store[playerId];
                SetTrapsPlayerInfo(playerId, playerSelectedTraps);
                //--UIも更新
                gui_m.SetTrapIconSprite(playerId, playerSelectingTrap_store[playerId], playerSelectingTrap_mine[playerId]);

                isStoreTrapTable[playerId] = false;
                //UIを更新
                gui_m.SetTrapSelectCursor(
                    playerId,
                    playerSelectingTrap_mine[playerId],
                    isStoreTrapTable[playerId]
                    );

                //全員準備が終わったかの確認
                if (CheckAllSelected())
                {
                    //レース始められるフラグをオン
                    isLadyGame = true;
                    //最終確認お化けアニメーション
                    gui_m.PlayLastCheckSignBoardAnim(true);
                }
            }
            //自分のトラップ選択中なら一覧に移動
            else
            {
                isStoreTrapTable[playerId] = true;
                //UIを更新
                gui_m.SetTrapSelectCursor(
                    playerId,
                    playerSelectingTrap_store[playerId],
                    isStoreTrapTable[playerId]
                    );

                //レース始められるフラグをオフ
                isLadyGame = false;

                //最終確認お化けアニメーション
                gui_m.PlayLastCheckSignBoardAnim(false);
            }

        }
    }

    /// <summary>
    /// キャンセルボタン
    /// </summary>
    public void Cancel(int playerId)
    {
        //ゲーム開始後は受け付けない
        if (isStartGame) return;

        //何かしらの遷移アニメーション中は受け付けない
        if (isPlayingAnim) return;

        //タイトル画面
        if (nowPhase == NowPhase.Title)
        {
            //何もなし
        }
        //人数選択画面
        if(nowPhase == NowPhase.CountSelect)
        {
            CountSelectView(isReturn: true);
        }
        //キャラ選択画面
        if (nowPhase == NowPhase.CharaSelect)
        {
            if (playerIsLady[playerId])
            {
                //準備状態を解除
                playerIsLady[playerId] = false;
                gui_m.ViewCharactorImage(playerId, false);
            }
            else
            {
                CharactorSelectView(isReturn: true);
            }
        }
        //トラップ選択画面
        if (nowPhase == NowPhase.TrapSelect)
        {
            //一覧選択中なら自分のトラップ選択へ
            if (isStoreTrapTable[playerId])
            {
                isStoreTrapTable[playerId] = false;
                //UIを更新
                gui_m.SetTrapSelectCursor(
                    playerId,
                    playerSelectingTrap_mine[playerId],
                    isStoreTrapTable[playerId]
                    );
            }
            ////準備をキャンセル
            //playerIsLady[playerId] = false;
        }
    }

    /// <summary>
    /// カーソルを移動
    /// </summary>
    /// <param name="playerId"></param>
    /// <param name="next"></param>
    public void OnMove(int playerId, Vector2 vec)
    {
        print(vec+":P"+playerId);
        //ゲーム開始後は受け付けない
        if (isStartGame) return;

        //何かしらの遷移アニメーション中は受け付けない
        if (isPlayingAnim) return;

        //タイトル画面
        if (nowPhase == NowPhase.Title)
        {
            //特になし
        }
        //人数選択画面
        else if(nowPhase == NowPhase.CountSelect)
        {
            //横入力がない場合は返却
            if ((int)vec.x == 0) return;
            //まだ変更アニメーション中は返却
            if (countChangingFlag) return;

            //範囲内で値を変更
            maxPlayerCount += (int)vec.x;
            if(maxPlayerCount < 1 || maxPlayerCount > 4)
            {
                maxPlayerCount = Mathf.Clamp(maxPlayerCount, 1, 4);
                return;
            }
            //UIを更新
            gui_m.ChangePlayerCountText((int)vec.x, maxPlayerCount);
        }
        //キャラ選択画面
        else if (nowPhase == NowPhase.CharaSelect)
        {
            //すでに準備できている状態は返却
            if (playerIsLady[playerId]) return;
            if (charaChangingFlag[playerId]) return;

            //横入力がない場合は返却
            if ((int)vec.x == 0) return;

            //順番にキャラを表示
            playerCharactor[playerId] = (playerCharactor[playerId] + (int)vec.x + 4) % 4;
            gui_m.ChangePlayingCharactorsImage(playerId, playerCharactor[playerId]);
            charaChangingFlag[playerId] = true;
        }
        //トラップ選択画面
        else if (nowPhase == NowPhase.TrapSelect)
        {
            //一覧選択中
            if (isStoreTrapTable[playerId])
            {
                //左右移動
                if (vec.x > 0)
                {
                    //一つ右に移動
                    playerSelectingTrap_store[playerId]++;
                    //右に移動できないと左端にループ
                    for (int i = 1; i <= trapHighCount; i++)
                    {
                        if (playerSelectingTrap_store[playerId] == trapWidthCount * i)
                        {
                            playerSelectingTrap_store[playerId] = trapWidthCount * i - 4;
                            break;
                        }
                    }
                }
                else if (vec.x < 0)
                {
                    //一つ左に移動
                    playerSelectingTrap_store[playerId]--;
                    //左に移動できないと右端にループ
                    for (int i = 0; i < trapHighCount; i++)
                    {
                        if (playerSelectingTrap_store[playerId] == trapWidthCount * i - 1)
                        {
                            playerSelectingTrap_store[playerId] = trapWidthCount * (i + 1) - 1;
                            break;
                        }
                    }
                }
                //上下移動
                if (vec.y > 0)
                {
                    //一つ上に移動
                    playerSelectingTrap_store[playerId] -= trapWidthCount;
                    //上に移動できないと下端にループ
                    for (int i = 0; i < trapWidthCount; i++)
                    {
                        if (playerSelectingTrap_store[playerId] == i - trapWidthCount)
                        {
                            playerSelectingTrap_store[playerId] += trapWidthCount * trapHighCount;
                            break;
                        }
                    }
                }
                else if (vec.y < 0)
                {
                    //一つ下に移動
                    playerSelectingTrap_store[playerId] += trapWidthCount;
                    //下に移動できないと上端にループ
                    for (int i = 0; i < trapWidthCount; i++)
                    {
                        if (playerSelectingTrap_store[playerId] == trapWidthCount * trapHighCount + i)
                        {
                            playerSelectingTrap_store[playerId] -= trapWidthCount * trapHighCount;
                            break;
                        }
                    }
                }
                //UIを更新
                gui_m.SetTrapSelectCursor(
                    playerId,
                    playerSelectingTrap_store[playerId],
                    isStoreTrapTable[playerId]
                    );
            }
            //自分のトラップ選択中
            else
            {
                //右入力
                if (vec.x > 0)
                {
                    playerSelectingTrap_mine[playerId] = 1;
                }
                //左入力
                else if (vec.x < 0)
                {
                    playerSelectingTrap_mine[playerId] = 3;
                }
                //上入力
                else if (vec.y > 0)
                {
                    playerSelectingTrap_mine[playerId] = 0;
                }
                //下入力
                else if (vec.y < 0)
                {
                    playerSelectingTrap_mine[playerId] = 2;
                }

                print(playerId + ":" + vec.x + ":" + playerSelectingTrap_mine[playerId]);
                //UIを更新
                gui_m.SetTrapSelectCursor(
                    playerId,
                    playerSelectingTrap_mine[playerId],
                    isStoreTrapTable[playerId]
                    );
            }
        }
    } 

    /// <summary>
    /// ゲーム開始ボタン
    /// </summary>
    public void OnStart()
    {
        //準備ができていたらゲーム開始
        if (isLadyGame)
        {
            isStartGame = true;
            StartGame();
        }
    }

    #endregion
}
