using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveDestroy : MonoBehaviour
{
    public ShockWaveTrapSc shockWave;

    public void FinishAnim()
    {
        shockWave.TimeUp();
    }
}
