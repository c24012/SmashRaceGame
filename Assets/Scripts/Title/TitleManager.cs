using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    int[] playerCharactor = new int[4];             //各プレイヤーのキャラクターId
    int[,] playerSelectedTraps = new int[4, 4];     //各プレイヤーのトラップId
    int[] playerSelectingTrap_store = new int[4];   //トラップ選択画面のカーソルの場所(トラップ一覧)
    int[] playerSelectingTrap_mine = new int[4];    //トラップ選択画面のカーソルの場所(自分のトラップ)
    bool[] isStoreTrapTable = new bool[4];          //自分のテーブルを選択中か一覧のテーブルを選択中か
    bool[] playerIsLady = new bool[4];              //プレイヤーの準備状態

    //[NonSerialized]
    public bool[] charaSelectFlag = new bool[4];

    public enum NowPhase
    {
        Title,
        CharaSelect,
        TrapSelect,
    }
    public NowPhase nowPhase = NowPhase.Title;      //現在のフェーズ

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            StartGame();
        }
    }

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
        return count >= currentPlayerCount;
    }

    /// <summary>
    /// トラップ選択画面へ
    /// </summary>
    public void ChengedToTrapPanel()
    {
        nowPhase = NowPhase.TrapSelect;
        //トラップ選択画面を表示
        gui_m.ViewTrapSelect();
    }

    /// <summary>
    /// タイトル画面から選択画面へと移動
    /// </summary>
    public void CharactorSelectView()
    {
        nowPhase = NowPhase.CharaSelect;
        //キャラ選択画面を表示
        gui_m.ViewCharactorSelect();
    }

    /// <summary>
    /// 選択画面画面からタイトルに戻る
    /// </summary>
    public void ReturnTitle()
    {
        nowPhase = NowPhase.Title;
        //タイトル画面を表示
        gui_m.ViewTitle();
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

        //--ここで画面の暗転アニメーションが入る(まだ)
        gui_m.PlayFadeIn();
        
    }

    public void FinishTitleFadeIn()
    {
        //レースシーンへ
        SceneManager.LoadScene("RaceScene");
    }

    /// <summary>
    /// ゲーム終了
    /// </summary>
    public void QuitGame()
    {
        //アプリケーションを終了
        Application.Quit();
    }

    #region プレイヤーからの入力

    /// <summary>
    /// 決定ボタン
    /// </summary>
    public void Decision(int playerId)
    {
        //タイトル
        if (nowPhase == NowPhase.Title)
        {
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

                //--カメラアニメーション(まだ)

                //トラップ選択画面へ
                Invoke(nameof(ChengedToTrapPanel), 1);
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
            }

        }
    }

    /// <summary>
    /// キャンセルボタン
    /// </summary>
    public void Cancel(int playerId)
    {
        print("B!");
        //タイトル画面
        if (nowPhase == NowPhase.Title)
        {
            //--長押ししたらゲーム終了(まだ)
            
        }
        //キャラ選択画面
        if (nowPhase == NowPhase.CharaSelect)
        {
            //押したプレイヤーのオブジェクトを破壊(コントローラー切断の為)
            Destroy(playerInfoList.Find((x) => x.playerIndex == playerId).playerInput.gameObject);
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
        //タイトル画面
        if (nowPhase == NowPhase.Title)
        {
            //特になし
        }
        //キャラ選択画面
        else if (nowPhase == NowPhase.CharaSelect)
        {
            //すでに準備できている状態は返却
            if (playerIsLady[playerId]) return;
            if (charaSelectFlag[playerId]) return;

            //順番にキャラを表示
            playerCharactor[playerId] = (playerCharactor[playerId] + (int)vec.x + 4) % 4;
            gui_m.ChangePlayingCharactorsImage(playerId, playerCharactor[playerId]);
            charaSelectFlag[playerId] = true;
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
                if(vec.x > 0)
                {
                    playerSelectingTrap_mine[playerId] = 1;
                }
                //左入力
                else if(vec.x < 0)
                {
                    playerSelectingTrap_mine[playerId] = 3;
                }
                //上入力
                else if(vec.y > 0)
                {
                    playerSelectingTrap_mine[playerId] = 0;
                }
                //下入力
                else if (vec.y < 0)
                {
                    playerSelectingTrap_mine[playerId] = 2;
                }

                //UIを更新
                gui_m.SetTrapSelectCursor(
                    playerId,
                    playerSelectingTrap_mine[playerId],
                    isStoreTrapTable[playerId]
                    );
            }
        }
    } 

    #endregion
}
