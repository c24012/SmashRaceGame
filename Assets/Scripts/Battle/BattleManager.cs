using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    [SerializeField, Header("デバッグモード")] bool debugMode;
    [SerializeField] int bebug_playerCount;
    //ゲームの情報取得用
    [SerializeField, Header("ゲームの情報")] GameData gameData;
    //プレイヤー人数
    int playerCount;
    //残り人数
    int remainingAmount;
    //生成用プレイヤープレハブ
    [SerializeField] GameObject[] playerPrefabs = new GameObject[4];
    //各プレイヤーオブジェクト
    List<GameObject> playerObjs = new();
    //プレイヤーの情報
    public List<PlayerData> playerDatas;
    //道の情報
    [SerializeField] CorseCheck corseCheck;
    //トラップストア
    [SerializeField] TrapStore trapStore;
    //一時停止画面
    [SerializeField] Canvas pauseMenuCanvas;
    //ピンぼけ
    [SerializeField] PostProcessVolume post;
    //誰が止めたか
    [SerializeField] TextMeshProUGUI pausePlayerText;
    //各キャラクターの色
    [SerializeField] Color32[] charaColor;
    //キャラの生成場所
    [SerializeField] Transform[] spownPoints;
    //ダミープレイヤーオブジェクト
    [SerializeField] List<GameObject> dummyPlayerObjs = new();

    //看板キャラの変数
    [SerializeField] GameObject[] signboardObj;
    [SerializeField] Image[] soulsImage;
    [SerializeField] Sprite[] soulsSprite;
    [SerializeField] TextMeshProUGUI[] livesCoutTexts;
    [SerializeField] Animator[] turnAnim;

    //フェードインのアニメーション
    [SerializeField] Animator anim;

    PauseManager pause;
    TimeLineManager timeLine;

    private void Awake()
    {
        //コンポーネントを取得
        pause = GetComponent<PauseManager>();
        timeLine = GetComponent<TimeLineManager>();

        //ゲームデータから情報を取得
        playerCount = gameData.playerCount;

        List<PlayerInfo> playerInfo = gameData.playerInfoList;

        //人数分プレイヤーオブジェクトを生成
        for (int i = 0; i < playerCount; i++)
        {
            PlayerManager pm = null;
            //デバックモード
            if (debugMode)
            {
                if (i >= bebug_playerCount) break;
                GameObject playerObj = Instantiate(playerPrefabs[i]);
                //オブジェクトの登録
                playerObjs.Add(playerObj);
                pm = playerObj.GetComponent<PlayerManager>();
            }
            else
            {
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
                //トラップの登録
                PlayerController pc = player.GetComponent<PlayerController>();
                for (int t = 0; t < 4; t++)
                {
                    pc.trapObj[t] = trapStore.trapObjs[playerInfo[i].trapNum[t]];
                }
            }
            //外部スクリプトを渡す
            pm.corseCheck = corseCheck; //コースの情報
            pm.pause = pause;    //ポーズマネージャーの登録
            //モードを指定
            pm.nowMode = PlayerManager.GameMode.Battle;
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
        //看板のキャラを人数分用意する
        for (int i = 0; i < playerCount; i++)
        {
            signboardObj[i].SetActive(true);
            if (!debugMode) soulsImage[i].sprite = soulsSprite[gameData.playerInfoList[i].charactorNum];
        }
        for(int i = playerCount;i < 4; i++)
        {
            signboardObj[i].SetActive(false);
        }
        //初期の残り人数を設定
        remainingAmount = playerObjs.Count;
    }

    private void Start()
    {
        //デバックモード
        if (debugMode)
        {
            //さっさとレース開始
            StartRace();
            return;
        }
        //スタートカウントダウンを開始
        timeLine.Play_StartCountDonw();
    }

    private void FixedUpdate()
    {
        //プレイヤーデータ収集
        GetPlayerDatas();
    }

    /// <summary>
    /// 各プレイヤーの残機をもとに順位を調べてデータに代入
    /// </summary>
    void GetPlayerDatas()
    {

        for (int i = 0; i < playerDatas.Count; i++) 
        {
            //すでに負けたプレイヤーは無視
            if (playerDatas[i].lives == -1) continue;

            //表示テキストとデータ残機が一致する場合、無視
            if (livesCoutTexts[i].text == playerDatas[i].lives.ToString()) continue;
            //異なる場合は更新&アニメーション
            livesCoutTexts[i].text = Mathf.Max(playerDatas[i].lives, 0).ToString();
            //看板回転アニメーション
            turnAnim[i].SetTrigger("TurnTrigger");

            if (playerDatas[i].lives == 0)
            {
                playerDatas[i].lives = -1;
                //残り人数で順位を決める
                playerDatas[i].ranking = remainingAmount - 1;
                //残り人数を減らす
                remainingAmount--;
                //残り人数が一人になったらゲーム終了
                if (remainingAmount <= 1)
                {
                    //残った一人の順位を1位に指定
                    for(int j = 0; j < playerCount; j++)
                    {
                        if (playerDatas[j].lives > 0) playerDatas[j].ranking = 0;
                    }
                    //ゲーム終了アニメーション起動
                    PlayFinishAnimation();
                }
            }
        }
    }

    /// <summary>
    /// ダミーのオブジェクトをリストに追加＆削除
    /// </summary>
    /// <param name="dummy"></param>
    /// <param name="isAdd"></param>
    public void AddOrRemoveDummyPlayerObj(GameObject dummy, bool isAdd)
    {
        if (isAdd) dummyPlayerObjs.Add(dummy);
        else dummyPlayerObjs.Remove(dummy);
    }

    public void PlayFinishAnimation()
    {
        //ゴールアニメーション
        timeLine.Play_FinishFadeIn();
        //プレイヤー全員の終了関数を呼ぶ
        FinishRace();
    }

    /// <summary>
    /// レース開始
    /// </summary>
    public void StartRace()
    {
        //登録されているプレイヤーオブジェクトのレース開始関数を起動
        for (int i = 0; i < playerObjs.Count; i++)
        {
            playerObjs[i].GetComponent<PlayerManager>().playerController.StartRace();
        }
    }

    void FinishRace()
    {
        //登録されているプレイヤーオブジェクトのレース終了関数を起動
        for (int i = 0; i < playerObjs.Count; i++)
        {
            playerObjs[i].GetComponent<PlayerManager>().playerController.FinishRace();
        }
        //ダミーが存在するならダミーの終了関数も起動
        for (int i = 0; i < dummyPlayerObjs.Count; i++)
        {
            dummyPlayerObjs[i].GetComponent<PlayerManager>().playerController.FinishRace();
        }
    }

    /// <summary>
    /// リザルト画面をロード
    /// </summary>
    public void ToResultScene()
    {
        //プレイヤーはランキング順に並び替え
        playerDatas = playerDatas.OrderByDescending((x) => x.ranking).ToList();
        //各プレイヤーデータのランキングをゲームデータのランキングに入力
        for (int i = 0; i < playerDatas.Count; i++)
        {
            gameData.ranking[i] = playerDatas[i].playerNum;
        }
        SceneManager.LoadScene("ResultScene");
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
    /// 誰が止めたかをテキストで表示
    /// </summary>
    /// <param name="playerId"></param>
    public void SetPausePlayerText(int playerNum)
    {
        pausePlayerText.text = $"{playerNum + 1}P";
        pausePlayerText.color = charaColor[playerNum];
    }

    /// <summary>
    /// タイトル画面をロード
    /// </summary>
    public void ToTitleScene()
    {
        //タイトルシーンロード
        anim.SetTrigger("Load");
        //時間停止を解除
        Time.timeScale = 1;
    }
}
