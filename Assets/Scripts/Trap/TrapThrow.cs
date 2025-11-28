using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapThrow : TrapBase
{
    [SerializeField, Header("投げるオブジェクト")] GameObject throwObj;
    [SerializeField] AnimationCurve throwCurve;
    [SerializeField, Header("回転するか")] bool isRotation;
    [SerializeField] float rotationSpeed = 0;
    
    Vector2 mastarPos;      //主の場所
    Vector2 trapPos;        //トラップの配置場所
    float distance;         //2点間の距離
    float movementRate = 0; //進行度(0-1)
    bool isFin = false;     //着地したか

    float MOVE_SPEED = 1.5f;

    void Start()
    {
        //TrapBaseの代わりにPlayerManagerを取得
        rankingPower = pm.playerData.ranking;
        //始めは本体を非表示
        trapObj.SetActive(false);
        //投げられるSpriteObjを表示
        throwObj.SetActive(true);
        //トラップの配置場所を保存
        trapPos = transform.position;
        //主の場所を取得
        mastarPos = pm.playerController.transform.position;
        //2点間の距離を計算
        distance = Vector2.Distance(trapPos, mastarPos);
        //自分の場所を主の場所に一旦移動
        transform.position = mastarPos;
    }

    private void Update()
    {
        //着地しているなら何もしない
        if (isFin) return;
        ThrowItem();
    }

    /// <summary>
    /// 目標に向かって飛ばす
    /// </summary>
    void ThrowItem()
    {
        //進行度を更新
        movementRate += Time.deltaTime * MOVE_SPEED;
        //最大値「1」を超えないようにする
        movementRate = Mathf.Min(movementRate, 1);
        //今の進行度によって二つの場所を線補完する
        transform.position = Vector2.Lerp(mastarPos, trapPos, movementRate);
        //今の進行度からカーブを読み取ってサイズを変える
        throwObj.transform.localScale = Vector2.one * throwCurve.Evaluate(movementRate);
        //回転フラグがオンなら回転スピードを参照して回転
        if (isRotation) throwObj.transform.Rotate(0, 0, rotationSpeed * (Time.deltaTime * 100));

        //着地した場合
        if (movementRate == 1)
        {
            //終了フラグをオン
            isFin = true;
            //トラップ本体に着地したことを発火
            LandedTrap();
            //表示オブジェクトを交換
            throwObj.SetActive(false);
            trapObj.SetActive(true);
        }
    }

    /// <summary>
    /// 本体への着地発火用
    /// </summary>
    virtual protected void LandedTrap()
    {
        //Debug.LogError("本体の関数に発火できませんでした Name:"+gameObject.name);
    }
}
