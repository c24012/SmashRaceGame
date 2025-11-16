using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimeLineManager : MonoBehaviour
{
    [SerializeField] PlayableDirector startCountDonw;

    /// <summary>
    /// スタートカウントダウンを再生
    /// </summary>
    public void Play_StartCountDonw()
    {
        startCountDonw.Play();
    }
}
