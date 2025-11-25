using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    //ポーズマネージャー
    [SerializeField] PauseManager pauseManager;
    //生成用プレイヤープレハブ
    [SerializeField] GameObject[] playerPrefabs = new GameObject[4];
    //各プレイヤーオブジェクト
    [SerializeField] List<GameObject> playerObjs = new();
    //プレイヤーの情報
    public List<PlayerData> playerDatas;
    //道の情報
    [SerializeField] CorseCheck corseCheck;
    //トラップストア
    [SerializeField] TrapStore trapStore;
    //参加アクション
    [SerializeField] InputActionReference joinActionRef;
    //参加検知用Action
    InputAction joinInputAction;
    //初期スポーン地点
    [SerializeField] Transform[] spownPoint = new Transform[4];
    //落下用の復帰場所
    [SerializeField] Transform respownPoint;
    //一時停止画面
    [SerializeField] Canvas pauseMenuCanvas;
    //ピンぼけ
    [SerializeField] PostProcessVolume post;


    //現プレイヤー人数
    int currentPlayerCount = 0;
    //現在使われているデバイスリスト
    List<InputDevice> inputDeviceList = new();
    //最大参加可能人数
    const int MAX_PLAYER_COUNT = 4;                 

    private void Awake()
    {
        //ボタンを検知できるようにIAReferenceからInputActionを取得
        joinInputAction = joinActionRef.action;

        //ボタン検知を有効化
        joinInputAction.Enable();

        //参加ボタン入力時呼び出す関数を登録
        joinInputAction.started += OnJoin;
    }

    void DisableActions()
    {
        //ボタン検知を解除
        joinInputAction.Disable();
        //参加ボタン入力時呼び出す関数を登録
        joinInputAction.started -= OnJoin;
    }

    /// <summary>
    /// 参加した時にプレイヤーを生成
    /// </summary>
    /// <param name="context"></param>
    void OnJoin(InputAction.CallbackContext context)
    {
        //最大数以上は無視
        if (currentPlayerCount >= MAX_PLAYER_COUNT) return;
        //すでにいる時は無視
        if (inputDeviceList.Contains(context.control.device)) return;

        //コントローラーを付与して生成
        PlayerInput player = PlayerInput.Instantiate(
            prefab: playerPrefabs[currentPlayerCount],
            playerIndex: currentPlayerCount,
            pairWithDevice: context.control.device
        );

        //オブジェクトの登録
        GameObject playerObj = player.transform.parent.gameObject;
        playerObjs.Add(playerObj);
        playerObj.transform.GetChild(0).position = spownPoint[currentPlayerCount].position;
        PlayerManager pm = playerObj.GetComponent<PlayerManager>();

        //外部スクリプトを渡す
        pm.corseCheck = corseCheck; //コースの情報の登録
        pm.pause = pauseManager;    //ポーズマネージャーの登録

        //プレイヤーデータを作成
        pm.playerData = new PlayerData(pm.playerNum, player.transform);
        pm.playerData.ranking = 4;                          //順位を5位固定
        pm.playerData.nearestPos = respownPoint.position;   //復帰場所を固定
        playerDatas.Add(pm.playerData);

        //登録したコントローラーを登録
        inputDeviceList.Add(context.control.device);
        //現在の参加者を増加
        currentPlayerCount++;

        //レース開始関数を呼ぶ
        pm.playerController.StartRace();
    }

    /// <summary>
    /// ポーズメニューを表示&非表示
    /// </summary>
    /// <param name="isActive"></param>
    public void ViewPauseMenu(bool isActive)
    {
        if (isActive)
        {
            pauseMenuCanvas.enabled = true;
            post.weight = 1;
            Time.timeScale = 0;
        }
        else
        {
            pauseMenuCanvas.enabled = false;
            post.weight = 0;
            Time.timeScale = 1;
        }
    }

    /// <summary>
    /// タイトル画面をロード
    /// </summary>
    public void ToTitleScene()
    {
        //ボタン検知を解除
        DisableActions();
        SceneManager.LoadScene("TitleScene");
        //時間停止を解除
        Time.timeScale = 1;
    }
}
