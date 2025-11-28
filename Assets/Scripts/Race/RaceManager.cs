using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using UnityEngine.Splines;
using UnityEngine.UI;

[Serializable]
/// <summary>
/// プレイヤーのレース情報データ
/// </summary>
public class PlayerData
{
    public int playerNum;       //番号
    public Transform newTf;     //現在位置
    public int lapCount;        //周回数
    public int ranking;         //順位
    public Vector3 nearestPos;  //最寄りルート(復帰場所)
    public float percentagePos; //現在地の割合
    public int progress;        //進行度
   
    public PlayerData(int playerNum, Transform newTf)
    {
        this.playerNum = playerNum;
        this.newTf = newTf;
        lapCount = 0;
        ranking = 0;
        nearestPos = Vector3.zero;
        percentagePos = 0;
        progress = 0;
    }
}

public class RaceManager : MonoBehaviour
{
    [SerializeField,Header("デバッグモード")] bool debugMode;
    [SerializeField] int bebug_playerCount;
    //ゲームの情報取得用
    [SerializeField,Header("")] GameData gameData;
    //プレイヤー人数
    [SerializeField] int playerCount;
    //必要周回回数
    public int lapCount;
    //生成用プレイヤープレハブ
    [SerializeField] GameObject[] playerPrefabs = new GameObject[4];
    //各プレイヤーオブジェクト
    [SerializeField] List<GameObject> playerObjs = new();
    //プレイヤーの情報
    public List<PlayerData> playerDatas;
    //道のスプライン
    [SerializeField] SplineContainer roadSpline;
    //道の情報
    [SerializeField] CorseCheck corseCheck;
    //トラップストア
    [SerializeField] TrapStore trapStore;
    //一時停止画面
    [SerializeField] Canvas pauseMenuCanvas;
    //ピンぼけ
    [SerializeField] PostProcessVolume post;

    // 解像度
    [SerializeField, Range(SplineUtility.PickResolutionMin, SplineUtility.PickResolutionMax)]
    private int resolution = 4;
    // 計算回数
    [SerializeField, Range(1, 10)]
    private int iterations = 2;

    //看板キャラのオブジェ
    [SerializeField] GameObject[] charaObj;
    [SerializeField] Image[] charaImage;
    [SerializeField] Sprite[] cahraSp;
    [SerializeField] TextMeshProUGUI[] lapText;

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
        for (int i = 0;i < playerCount; i++)
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
        }
        //プレイヤー全員のデータを生成
        for(int i = 0; i < playerObjs.Count; i++)
        {
            Transform charactor = playerObjs[i].transform.GetChild(0);
            PlayerManager pm = playerObjs[i].GetComponent<PlayerManager>();
            pm.playerData = new PlayerData(pm.playerNum, charactor);
            playerDatas.Add(pm.playerData);
        }
        //看板のキャラを人数分用意する
        for(int i = 0; i < playerCount; i++)
        {
            charaObj[i].SetActive(true);
            if(!debugMode)charaImage[i].sprite = cahraSp[gameData.playerInfoList[i].charactorNum];
            lapText[i].text = 0 + "/" + lapCount;
        }
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

    private void Update()
    {
        //プレイヤーデータ収集
        GetPlayerDatas();
    }

    /// <summary>
    /// 各プレイヤーの順位や周回を調べてデータに代入
    /// </summary>
    void GetPlayerDatas()
    {
        // ワールド空間におけるスプラインを取得
        // スプラインはローカル空間なので、ローカル→ワールド変換行列を掛ける
        // Updateを抜けるタイミングでDisposeされる
        using NativeSpline spline = new(roadSpline.Spline, roadSpline.transform.localToWorldMatrix);

        for (int i = 0; i < playerObjs.Count; i++)
        {
            // スプラインにおける直近位置を求める
            Single distance = SplineUtility.GetNearestPoint(
                spline,
                playerDatas[i].newTf.position,
                out float3 nearest,
                out float t,
                resolution,
                iterations
            );

            //最も近いルートを取得
            playerDatas[i].nearestPos = new Vector2(nearest.x, nearest.y);
            //2割以下の進行は進行度を進ませる
            if (playerDatas[i].progress < (int)(t * 10))
            {
                if ((int)(t * 10) - playerDatas[i].progress <= 2)
                {
                    playerDatas[i].progress = (int)(t * 10);
                }
            }
            //今何週目のどこにいるかを取得
            playerDatas[i].percentagePos = t + playerDatas[i].lapCount;
            if (playerDatas[i].lapCount <= 0)
            {
                lapText[i].text = 0 + "/" + lapCount;
            }
            else
            {
                lapText[i].text = playerDatas[i].lapCount - 1 + "/" + lapCount;
            }
        }
        //リストをランキング順に並び替え
        playerDatas = playerDatas.OrderByDescending((x) => x.percentagePos).ToList();
        //各プレイヤーデータに現在のランキングを入力
        for (int i = 0; i < playerDatas.Count; i++)
        {
            playerDatas[i].ranking = i;
        }
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
        for(int i = 0;i< playerObjs.Count; i++)
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
    }

    /// <summary>
    /// リザルト画面をロード
    /// </summary>
    public void ToResultScene()
    {
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
    /// タイトル画面をロード
    /// </summary>
    public void ToTitleScene()
    {
        //タイトルシーンロード
        SceneManager.LoadScene("TitleScene");
        //時間停止を解除
        Time.timeScale = 1;
    }
}
