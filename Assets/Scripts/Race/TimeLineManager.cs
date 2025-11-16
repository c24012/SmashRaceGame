using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimeLineManager : MonoBehaviour
{
    [SerializeField] PlayableDirector startCountDonw;

    public void Play_StartCountDonw()
    {
        startCountDonw.Play();
    }
}
