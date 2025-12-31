using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.PostProcessing;

public class TutorialManager : MonoBehaviour
{
    //ゲームの情報取得用
    [SerializeField, Header("ゲームの情報")] GameData gameData;
    //プレイヤー人数
    int playerCount;
    //ポーズマネージャー
    [SerializeField] PauseManager pause;
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
    //落下用の復帰場所
    [SerializeField] Transform respownPoint;
    //一時停止画面
    [SerializeField] Canvas pauseMenuCanvas;
    //ピンぼけ
    [SerializeField] PostProcessVolume post;
    //フェードインのアニメーション
    [SerializeField] Animator anim;
    //キャラの生成場所
    [SerializeField] Transform[] spownPoints;

    private void Awake()
    {
        //コンポーネントを取得
        pause = GetComponent<PauseManager>();

        //ゲームデータから情報を取得
        playerCount = gameData.playerCount;

        List<PlayerInfo> playerInfo = gameData.playerInfoList;

        //人数分プレイヤーオブジェクトを生成
        for (int i = 0; i < playerCount; i++)
        {
            PlayerManager pm = null;
            //コントローラーを付与して生成
            PlayerInput player = PlayerInput.Instantiate(
                prefab: playerPrefabs[playerInfo[i].charactorNum],
                playerIndex: playerInfo[i].playerIndex,
                pairWithDevice: playerInfo[i].device
            );
            //オブジェクトの登録
            playerObjs.Add(player.transform.parent.gameObject);
            pm = player.transform.parent.GetComponent<PlayerManager>();
            //初期位置に移動
            Transform charactorTf = playerObjs[i].transform.GetChild(0);
            charactorTf.position = spownPoints[i].position;
            charactorTf.rotation = spownPoints[i].rotation;
            //外部スクリプトを渡す
            pm.corseCheck = corseCheck; //コースの情報
            pm.pause = pause;    //ポーズマネージャーの登録
            //モードを指定
            pm.nowMode = PlayerManager.GameMode.Battle;

            pm.playerController.StartRace();
        }
        //プレイヤー全員のデータを生成
        for (int i = 0; i < playerObjs.Count; i++)
        {
            Transform charactor = playerObjs[i].transform.GetChild(0);
            PlayerManager pm = playerObjs[i].GetComponent<PlayerManager>();
            pm.playerData = new PlayerData(pm.playerNum, charactor);
            pm.playerData.nearestPos = spownPoints[i].position;
            pm.playerData.ranking = 4;
            playerDatas.Add(pm.playerData);
        }
    }

    //private void Awake()
    //{
    //    //ボタンを検知できるようにIAReferenceからInputActionを取得
    //    joinInputAction = joinActionRef.action;

    //    //ボタン検知を有効化
    //    joinInputAction.Enable();

    //    //参加ボタン入力時呼び出す関数を登録
    //    joinInputAction.started += OnJoin;
    //}

    //void DisableActions()
    //{
    //    //ボタン検知を解除
    //    joinInputAction.Disable();
    //    //参加ボタン入力時呼び出す関数を登録
    //    joinInputAction.started -= OnJoin;
    //}

    ///// <summary>
    ///// 参加した時にプレイヤーを生成
    ///// </summary>
    ///// <param name="context"></param>
    //void OnJoin(InputAction.CallbackContext context)
    //{
    //    //最大数以上は無視
    //    if (currentPlayerCount >= MAX_PLAYER_COUNT) return;
    //    //すでにいる時は無視
    //    if (inputDeviceList.Contains(context.control.device)) return;

    //    //コントローラーを付与して生成
    //    PlayerInput player = PlayerInput.Instantiate(
    //        prefab: playerPrefabs[currentPlayerCount],
    //        playerIndex: currentPlayerCount,
    //        pairWithDevice: context.control.device
    //    );

    //    //オブジェクトの登録
    //    GameObject playerObj = player.transform.parent.gameObject;
    //    playerObjs.Add(playerObj);
    //    playerObj.transform.GetChild(0).position = spownPoint[currentPlayerCount].position;
    //    PlayerManager pm = playerObj.GetComponent<PlayerManager>();

    //    //外部スクリプトを渡す
    //    pm.corseCheck = corseCheck; //コースの情報の登録
    //    pm.pause = pauseManager;    //ポーズマネージャーの登録

    //    //プレイヤーデータを作成
    //    pm.playerData = new PlayerData(pm.playerNum, player.transform);
    //    pm.playerData.ranking = 4;                          //順位を5位固定
    //    pm.playerData.nearestPos = respownPoint.position;   //復帰場所を固定
    //    playerDatas.Add(pm.playerData);

    //    //登録したコントローラーを登録
    //    inputDeviceList.Add(context.control.device);
    //    //現在の参加者を増加
    //    currentPlayerCount++;

    //    //レース開始関数を呼ぶ
    //    pm.playerController.StartRace();
    //}

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
    /// フェードインアニメーションを起動
    /// </summary>
    public void ToTitleScene()
    {
        anim.SetTrigger("Load");
        //時間停止を解除
        Time.timeScale = 1;
    }
}
