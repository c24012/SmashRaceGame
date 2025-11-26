using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class JoinPlayerManager : MonoBehaviour
{
    PlayerInputManager playerInputManager;

    [SerializeField, Header("タイトルマネージャー")] TitleManager title_m;
    [SerializeField, Header("GUIマネージャー")] TitleGUIManager gui_m;
    [SerializeField, Header("参加アクション")] InputActionReference joinActionRef;
    [SerializeField, Header("チュートリアルアクション")] InputActionReference tutorialActionRef;
    [SerializeField, Header("タイトル操作プレイヤーPrefab")] PlayerInput playerObj;

    [SerializeField] GameObject firstSelectObj;
    [SerializeField] GameObject cursorRootObj;

    InputAction joinInputAction;                    //参加ボタン検知用
    InputAction tutorialInputAction;                //チュートリアルボタン検知用
    List<InputDevice> inputDeviceList = new();      //現在使われているデバイスリスト
    const int MAX_PLAYER_COUNT = 4;                 //最大参加可能人数


    private void Awake()
    {
        //PlayerInputManagerを取得
        playerInputManager = GetComponent<PlayerInputManager>();

        //参加ボタン,チュートリアルボタンを検知できるようにIAReferenceからInputActionを取得
        joinInputAction = joinActionRef.action;
        tutorialInputAction = tutorialActionRef.action;

        //ボタン検知を有効化
        joinInputAction.Enable();
        tutorialInputAction.Enable();
        //参加ボタン入力時呼び出す関数を登録
        joinInputAction.started += OnJoin;
        tutorialInputAction.started += OnTutorial;
        //参加時に呼ばれる関数を登録
        playerInputManager.onPlayerJoined += OnJoinManager;
        //オブジェクトが破壊されたときに呼ばれる関数を登録
        playerInputManager.onPlayerLeft += OnLeaveManager;
    }

    public void DisableActions()
    {
        //ボタン検知を解除
        joinInputAction.Disable();
        tutorialInputAction.Disable();
        //参加ボタン入力時呼び出す関数を登録
        joinInputAction.started -= OnJoin;
        tutorialInputAction.started -= OnTutorial;
        //参加時に呼ばれる関数を登録
        playerInputManager.onPlayerJoined -= OnJoinManager;
        //オブジェクトが破壊されたときに呼ばれる関数を登録
        playerInputManager.onPlayerLeft -= OnLeaveManager;
    }

    /// <summary>
    /// プレイヤーが生成された時に呼ばれる
    /// </summary>
    /// <param name="input"></param>
    void OnJoinManager(PlayerInput input)
    {
        
    }

    /// <summary>
    /// プレイヤーが破壊されたときに呼ばれる
    /// </summary>
    /// <param name="input"></param>
    private void OnLeaveManager(PlayerInput input)
    {
        if (title_m.nowPhase == TitleManager.NowPhase.TrapSelect) return;
        //プレイヤー詳細データから削除
        title_m.playerInfoList.Remove(title_m.playerInfoList.Find((x) => x.playerInput == input));
        //デバイスリストから破壊されたプレイヤーのデバイスを削除
        inputDeviceList.Remove(input.devices[0]);
        //現在の参加者を減らす
        title_m.currentPlayerCount--;
        //GUI表示を更新
        //gui_m.SetPlayerPanel(title_m.currentPlayerCount);
        //プレイヤー詳細データを更新
        title_m.PlayerInfoDataUpdate();
    }

    /// <summary>
    /// 参加ボタンが押されたコントローラーからプレイヤーを生成
    /// </summary>
    /// <param name="context"></param>
    void OnJoin(InputAction.CallbackContext context)
    {
        //最大数以上は無視
        if (title_m.currentPlayerCount >= MAX_PLAYER_COUNT) return;
        //すでにいる時は無視
        if (inputDeviceList.Contains(context.control.device)) return;
        //コントローラーを付与して生成
        PlayerInput player = PlayerInput.Instantiate(
            prefab: playerObj.gameObject,
            playerIndex: title_m.currentPlayerCount,
            pairWithDevice: context.control.device
        );

        //登録したコントローラーを登録
        inputDeviceList.Add(context.control.device);
        //現在の参加者を増加
        title_m.currentPlayerCount++;
        //プレイヤーを登録
        title_m.playerInfoList.Add(new PlayerInfo(player.playerIndex,player,context.control.device));

        //参加ボタンで人数選択画面へ
        if(title_m.nowPhase == TitleManager.NowPhase.Title)
        {
            title_m.CountSelectView();
        }

        //GUI表示を更新
        gui_m.SetJoinPlayer(title_m.currentPlayerCount);
    }

    void OnTutorial(InputAction.CallbackContext context)
    {
        title_m.StartTutorial();
    }
}
