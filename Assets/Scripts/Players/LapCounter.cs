using UnityEngine;

public class LapCounter : MonoBehaviour
{
    PlayerManager pm;
    [SerializeField,Header("確認用")] int before;

    private void Awake()
    {
        //プレイヤーマネージャー取得
        pm = transform.transform.parent.GetComponent<PlayerManager>();
    }

    void Start()
    {
        before = pm.playerData.progress;
    }

    void Update()
    {
        CheckLaps();
    }

    void CheckLaps()
    {
        if ((int)((pm.playerData.percentagePos - pm.playerData.lapCount) * 10) == 0)
        {
            if (before == 9)
            {
                pm.playerData.progress = 0;
                pm.playerData.lapCount++;
            }
        }
        if ((int)((pm.playerData.percentagePos - pm.playerData.lapCount) * 10) == 9)
        {
            if (before == 0)
            {
                pm.playerData.progress = 9;
                pm.playerData.lapCount = Mathf.Max(--pm.playerData.lapCount, 0);
            }
        }
        before = Mathf.Clamp((int)((pm.playerData.percentagePos - pm.playerData.lapCount) * 10),0,9);
    }
}
