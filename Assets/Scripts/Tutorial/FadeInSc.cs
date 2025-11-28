using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeInSc : MonoBehaviour
{
    /// <summary>
    /// タイトル画面をロード
    /// </summary>
    public void ToTitleScene()
    {
        SceneManager.LoadScene("TitleScene");
    }
}
