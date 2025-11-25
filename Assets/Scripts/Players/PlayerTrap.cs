using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PlayerTrap : MonoBehaviour
{
    PlayerManager pm;

    [SerializeField] GameObject aimObj;

    [SerializeField, Tooltip("Rayの長さ")] float maxPower;
    [SerializeField, Tooltip("置くトラップの種類の番号")] int trapNum = 0;
    [SerializeField] float chargeSpeed = 1;
    [SerializeField] float power = 0;
    [SerializeField] float coolTime = 4f;

    public bool trapFlag = true;

    bool isCharge = false;

    private void Awake()
    {
        //PlayerManagerを取得
        pm = transform.transform.parent.GetComponent<PlayerManager>();

        trapFlag = true;
        Init();
    }

    /// <summary>
    /// 初期化
    /// </summary>
    void Init()
    {
        isCharge = false;
        power = 0.5f;
        aimObj.SetActive(false);
        aimObj.transform.localPosition = Vector3.zero;
    }

    private void Update()
    {
        //チャージフラグがONなら溜め始める
        if (isCharge) ChargeThrowPower();
    }

    /// <summary>
    /// 力を溜める＆UI更新
    /// </summary>
    void ChargeThrowPower()
    {
        //少しずつ溜まる
        power += Time.deltaTime * chargeSpeed;
        //上限を超えないようにする
        if (power > maxPower) power = maxPower;

        //トラップを置く直線状に障害物があるか確認用のレイ
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, power, 1);
        //なにもない
        if(hit.collider == null)
        {
            //照準位置を更新
            aimObj.transform.localPosition = new Vector2(0, power);
        }
        else
        {
            //照準位置を更新
            aimObj.transform.position = hit.point;
        }
    }

    /// <summary>
    /// チャージを始める
    /// </summary>
    public void StartCharge()
    {
        isCharge = true;
        aimObj.SetActive(true);
    }

    /// <summary>
    /// チャージ量に遠くに投げる
    /// </summary>
    public void StopCharge()
    {
        //チャージされていなかったらキャンセル
        if (!isCharge) return;
        //溜めた分の距離でトラップを配置
        Trap();
        //ゲージをリセット
        Init();
        aimObj.SetActive(false);
    }

    public void TrapChange(int value = 0,int trapIndex = -1)
    {
        //直接インデックスを指定
        if (trapIndex != -1)
        {
            trapNum = trapIndex;
        }
        else
        {
            //0～最大値までのループ計算
            trapNum = (trapNum + value + pm.playerController.trapObj.Length) % pm.playerController.trapObj.Length;
        }
        //トラップとアイコンを変更
        pm.iconManager.IconChange(trapNum);
        pm.iconManager.BanCheck(!trapFlag);

        //今貯めているパワーはリセット
        Init();
    }

    /// <summary>
    /// 罠を設置
    /// </summary>
    public void Trap()
    {
        bool isInstant = pm.playerController.trapObj[trapNum].GetComponent<TrapBase>().isInstantActive;
        //トラップを置いてから一定時間後にフラグを解除
        if (trapFlag)
        {
            trapFlag = false;
            Invoke(new Action(() => { trapFlag = true; pm.iconManager.BanCheck(!trapFlag); }).Method.Name, coolTime);
        }
        else
        {
            pm.iconManager.BanCheck(false);
            return;
        }

        //トラップを置く直線状に障害物があるか確認用のレイ
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, power, 1);

        //PlayerManager代入用
        TrapBase sampleBase = null;
        //壁に当たった場合
        if (hit.collider != null)
        {
            sampleBase = Instantiate(pm.playerController.trapObj[trapNum], hit.point, transform.rotation).
                GetComponent<TrapBase>();
        }
        //当たっていない場合
        else
        {
            sampleBase = Instantiate(pm.playerController.trapObj[trapNum],
                transform.position + transform.up * power, transform.rotation).
                GetComponent<TrapBase>();
        }
        sampleBase.pm = pm;
        pm.iconManager.BanCheck(!trapFlag);
    }

    public bool GetIsInstantActive()
    {
        return pm.playerController.trapObj[trapNum].GetComponent<TrapBase>().isInstantActive;
    }

    /// <summary>
    /// パワーゲージをリセット
    /// </summary>
    public void ResetCharge()
    {
        Init();
    }
}
