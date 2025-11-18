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
    public PlayerInput playerInput; //PlayerInputコンポーネント
    public int playerIndex;         //PlayerInputの登録番号
    public InputDevice device;      //使っているデバイス
    public int charactorNum;        //プレイヤーの選んだキャラ

    public PlayerInfo(int playerIndex, PlayerInput playerInput, InputDevice device, int selectCharactor = 0)
    {
        this.playerInput = playerInput;
        this.playerIndex = playerIndex;
        this.device = device;
        charactorNum = selectCharactor;
    }

    public void SetCharactor(int selectCharactor)
    {
        charactorNum = selectCharactor;
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
    int[,] playerSelectedTraps = new int[4,4];      //各プレイヤーのトラップId
    int[] playerSelectingTrap_store = new int[4];   //トラップ選択画面のカーソルの場所(トラップ一覧)
    int[] playerSelectingTrap_mine = new int[4];    //トラップ選択画面のカーソルの場所(自分のトラップ)
    bool[] isStoreTrapTable = new bool[4];           //自分のテーブルを選択中か一覧のテーブルを選択中か
    bool[] playerIsLady = new bool[4];              //プレイヤーの準備状態

    public enum NowPhase
    {
        Title,
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
        foreach(bool ready in playerIsLady) if (ready) count++;
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
    /// プレイヤーのキャラクターを指定
    /// </summary>
    /// <param name="playerIndex"></param>
    /// <param name="charaNum"></param>
    void SetCharactorPlayerInfo(int playerIndex, int charaNum)
    {
        playerInfoList.Find((x) => x.playerIndex == playerIndex).SetCharactor(charaNum);
    }

    /// <summary>
    /// 持っているプレイヤー詳細データを共有用データPrefabに保存
    /// </summary>
    void SetGameData()
    {
        //人数
        gameData.playerCount = currentPlayerCount;
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
        print("A!");
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
            gui_m.ViewCharactorImage(playerId,true);
            //プレイヤーのキャラをデータ型に代入
            SetCharactorPlayerInfo(playerId, playerCharactor[playerId]);
            //他のプレイヤーも準備ができたらトラップ選択画へ
            if (CheckAllReady())
            {

                //--カメラアニメーション(まだ)

                //トラップ選択画面へ
                ChengedToTrapPanel();
                //準備フラグを全員リセット
                for(int i = 0; i < 4; i++)
                {
                    playerIsLady[i] = false;
                }
            }
        }
        //トラップ画面
        else if (nowPhase == NowPhase.TrapSelect)
        {
            
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
        if(nowPhase == NowPhase.CharaSelect)
        {

        }
        //トラップ選択画面
        if (nowPhase == NowPhase.TrapSelect)
        {
            //準備をキャンセル
            playerIsLady[playerId] = false; 
        }
    }

    /// <summary>
    /// キャラを変更
    /// </summary>
    /// <param name="playerId"></param>
    /// <param name="next"></param>
    public void OnMove(int playerId,Vector2 vec)
    {
        print("十字入力");
        //タイトル画面
        if (nowPhase == NowPhase.Title)
        {
            //特になし
        }
        //キャラ選択画面
        if (nowPhase == NowPhase.CharaSelect)
        {
            //すでに準備できている状態は返却
            if (playerIsLady[playerId]) return;

            //順番にキャラを表示
            playerCharactor[playerId] = (playerCharactor[playerId] + (int)vec.x + 4) % 4;
            gui_m.ChangePlayingCharactorsImage(playerId, playerCharactor[playerId]);
        }
        //トラップ選択画面
        if (nowPhase == NowPhase.TrapSelect)
        {
            //一覧選択中
            if (isStoreTrapTable[playerId])
            {
                //左右移動
                if (vec.x > 0)
                {
                    //一つ右に移動
                    playerSelectingTrap_store[playerId]++;
                    //右に移動できないと左に戻す
                    for(int i = 1;i <= trapHighCount; i++)
                    {
                        if (playerSelectingTrap_store[playerId] == trapWidthCount * i)
                        {
                            playerSelectingTrap_store[playerId]--;
                        }
                    }
                    //UIを更新
                    gui_m.SetTrapSelectCursor(
                        playerId,
                        playerSelectingTrap_store[playerId],
                        isStoreTrapTable[playerId]
                        );
                }
                else if (vec.x < 0)
                {

                }
            }
        }
    }

    #endregion
}
