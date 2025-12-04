using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultManager : MonoBehaviour
{
    public Image[] charaImage;

    public Sprite[] charaSp;

    public GameData data;

    public PlayableDirector fadeOutAnim;

    [SerializeField] InputActionReference returnButton;
    InputAction returnAction;

    //フェードインのアニメーション
    [SerializeField] Animator anim;

    private void Awake()
    {
        //終了ボタンを有効化
        returnAction = returnButton.action;
        returnAction.Enable();
        returnAction.started += ReturnTitle;
    }
    private void Start()
    {

        for(int i = 3; i > data.playerCount - 1; i--)
        {
            charaImage[i].enabled = false;
        }

       for(int i = 0;i < data.playerCount; i++)
        {
            charaImage[i].sprite = charaSp[data.ranking[i] + (i * 4)];
        }

        fadeOutAnim.Play();
    }

    void ReturnTitle(InputAction.CallbackContext context)
    {
        
        if (context.started)
        {
            //タイトルシーンロード
            anim.SetTrigger("Load");
            returnAction.started -= ReturnTitle;
        }
    }
}
