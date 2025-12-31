using UnityEngine;

public class FPSManager : MonoBehaviour
{
    private void Awake()
    {
        Application.targetFrameRate = 120;
        Cursor.visible = false;
    }
}
