using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerGage : MonoBehaviour
{
    [SerializeField] PlayerManager playerManager;

    [SerializeField] Image gageImage;

    [SerializeField] float chargeSpeed = 1;
    [SerializeField] float power = 0;

    bool isCharge = false;

    private void Awake()
    {
        Init();
    }

    /// <summary>
    /// 初期化
    /// </summary>
    void Init()
    {
        isCharge = false;
        power = 0;
        gageImage.fillAmount = 0;
    }

    private void Update()
    {
        //チャージフラグがONなら溜め始める
        if (isCharge) ChargePower();
    }

    /// <summary>
    /// 力を溜める＆UI更新
    /// </summary>
    void ChargePower()
    {
        //少しずつ溜まる速度を上昇
        power += power * Time.deltaTime * 3.5f + Time.deltaTime / 5 * chargeSpeed;
        //上限の「1」を超えないようにする
        if (power > 1) power = 1;
        //PlayerのUIにも適応
        gageImage.fillAmount = power;
    }

    /// <summary>
    /// チャージを始める
    /// </summary>
    public void StartCharge()
    {
        isCharge = true;
    }

    /// <summary>
    /// チャージ量に応じて移動
    /// </summary>
    public void StopCharge()
    {
        //溜めたpowerをコントローラーに送る
        playerManager.playerController.Move(power);
        //ゲージをリセット
        Init();
    }
}
