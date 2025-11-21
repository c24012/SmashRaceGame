using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimeLineManager : MonoBehaviour
{
    [SerializeField] PlayableDirector startCountDonw;
    [SerializeField] PlayableDirector finishFadeIn;

    /// <summary>
    /// スタートカウントダウンを再生
    /// </summary>
    public void Play_StartCountDonw()
    {
        startCountDonw.Play();
    }

    /// <summary>
    /// ゴールアニメーションとフェードインを再生
    /// </summary>
    public void Play_FinishFadeIn()
    {
        finishFadeIn.Play();
    }
}
