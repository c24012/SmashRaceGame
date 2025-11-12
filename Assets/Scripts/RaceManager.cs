using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;

[Serializable]
/// <summary>
/// プレイヤーの情報クラス
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
    //ゲームの情報取得用
    [SerializeField] GameData gameData;
    //プレイヤー人数
    [SerializeField] int playerCount;
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

    // 解像度
    [SerializeField, Range(SplineUtility.PickResolutionMin, SplineUtility.PickResolutionMax)]
    private int resolution = 4;
    // 計算回数
    [SerializeField, Range(1, 10)]
    private int iterations = 2;


    private void Awake()
    {
        //ゲームデータから情報を取得
        playerCount = gameData.playerCount;

        //人数分プレイヤーオブジェクトを生成
        for (int i = 0;i < playerCount; i++)
        {
            PlayerManager pm = Instantiate(playerPrefabs[i]).transform.GetChild(0).GetComponent<PlayerManager>();
            //外部スクリプトを渡す
            pm.corseCheck = corseCheck; //コースの情報
        }
        //プレイヤー全員のデータを生成
        for(int i = 0; i < playerObjs.Count; i++)
        {
            Transform charactor = playerObjs[i].transform.GetChild(0);
            PlayerManager pm = charactor.GetComponent<PlayerManager>();
            pm.playerData = new PlayerData(pm.playerNum, charactor);
            playerDatas.Add(pm.playerData);
        }
    }

    private void Update()
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
            playerDatas[i].nearestPos = nearest;
            //今何週目のどこにいるかを取得
            playerDatas[i].percentagePos = t + playerDatas[i].lapCount;
            //2割以下の進行は進行度を進ませる
            if(playerDatas[i].progress < (int)(t * 10))
            {
                if((int)(t * 10) - playerDatas[i].progress <= 2)
                {
                    playerDatas[i].progress = (int)(t * 10);
                }
            }
        }

        playerDatas = playerDatas.OrderByDescending((x) => x.percentagePos).ToList();

        for (int i = 0; i < playerDatas.Count; i++)
        {
            playerDatas[i].ranking = i;
        }

    }

    //[ContextMenu("Sort")]
    //void Sort_test()
    //{
    //    playerDatas.Sort((x, y) => (int)(x.percentagePos - y.percentagePos));
    //    print("あ");
    //}
    //[ContextMenu("OrderBy")]
    //void OrderBy_test()
    //{
    //    playerDatas.OrderBy((x) => x.percentagePos);
    //}
}
