using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveGage : MonoBehaviour
{
    [SerializeField] PlayerManager playerManager;

    [SerializeField] Image bar;
    [SerializeField] Image middleGage;
    [SerializeField] Image highGage;

    [SerializeField] float barSpeed = 1;
    [SerializeField] int direction = 1;
    [SerializeField] float moveCoolTime = 1;

    [SerializeField, Range(0, 1)] float difficulty = 0;
    [SerializeField] float middle_MaxSize = 150; 
    [SerializeField] float middle_MinSize = 30; 
    [SerializeField] float high_MaxSize = 100; 
    [SerializeField] float high_MinSize = 10;

    [SerializeField] float middleGageSize;
    [SerializeField] float highGageSize;

    const int GAGE_SIZE = 200;
    const int BAR_SPEED_ADJUST = 200;

    bool isStop = false;

    private void Awake()
    {
        Init();
    }

    void Init()
    {
        //ゲージエリア初期化
        SetGageDifficulty();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S)) SetGageDifficulty();
        if (Input.GetKeyDown(KeyCode.R)) isStop = false;
        if (!isStop) MoveBar();
    }

    public void StopBar()
    {
        if (isStop) return;
        isStop = true;
        CheckWhereStop();
        Invoke(nameof(ResetBar), moveCoolTime);
    }

    void ResetBar()
    {
        isStop = false;
        bar.rectTransform.localPosition = new Vector2(-100, bar.rectTransform.localPosition.y);
    }

    /// <summary>
    /// バーの往復移動
    /// </summary>
    void MoveBar()
    {
        Vector2 pos = bar.rectTransform.localPosition;
        pos.x += barSpeed * Time.deltaTime * direction * BAR_SPEED_ADJUST;
        if (direction > 0 && pos.x > GAGE_SIZE / 2)
        {
            pos.x = GAGE_SIZE / 2;
            direction = -1;
        }
        else if (direction < 0 && pos.x < -(GAGE_SIZE / 2))
        {
            pos.x = -(GAGE_SIZE / 2);
            direction = 1;
        }
        bar.rectTransform.localPosition = pos;
    }

    /// <summary>
    /// ゲージの難易度
    /// </summary>
    void SetGageDifficulty()
    {
        //中ゲージの大きさを難易度に沿って変更(Rect.SizeDelta.x)
        Vector2 size = middleGage.rectTransform.sizeDelta;
        size.x = middle_MaxSize - (middle_MaxSize - middle_MinSize) * difficulty;
        middleGageSize = size.x;
        middleGage.rectTransform.sizeDelta = size;
        //大ゲージの大きさを難易度に沿って変更(Rect.SizeDelta.x)
        size = highGage.rectTransform.sizeDelta;
        size.x = high_MaxSize - (high_MaxSize - high_MinSize) * difficulty;
        highGageSize = size.x;
        highGage.rectTransform.sizeDelta = size;

        //バーの移動速度上昇
        barSpeed = 1 + difficulty;
    }

    /// <summary>
    /// どこで止めたか計算
    /// </summary>
    void CheckWhereStop()
    {
        float highBorder = highGageSize / 2;
        float middleBorder = middleGageSize / 2;
        float barX = Mathf.Abs(bar.rectTransform.localPosition.x);

        if (barX <= highBorder)
        {
            playerManager.playerController.Move(1);
        }
        else if(barX <= middleBorder)
        {
            playerManager.playerController.Move(0.5f);
        }
        else
        {
            playerManager.playerController.Move(0.2f);
        }
    }
}
