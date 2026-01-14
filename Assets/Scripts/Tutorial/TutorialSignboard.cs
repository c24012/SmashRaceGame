using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Video;

public class TutorialSignboard : MonoBehaviour
{
    [SerializeField,Header("コンポーネント")] VideoPlayer video;
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI info;
    [SerializeField] GameObject infoPanel;

    [SerializeField] TextMeshProUGUI itemName;
    [SerializeField] TextMeshProUGUI itemInfo;
    [SerializeField] Image itemIcon;
    [SerializeField] GameObject itemPanel;

    [SerializeField, Header("ページデータ")] TutorialSignboardInfo_SO[] pageDatas;
    [SerializeField, Header("ボタン入力")] InputActionReference pageControllRef;

    InputAction pageControllAction;
    int nowPage = 0;
    int maxPage = 0;

    private void Awake()
    {
        //ページめくり用入力を検知できるようにReferenceからInputActionを取得
        pageControllAction = pageControllRef.action;
        //ボタン検知を有効化
        pageControllAction.Enable();
        //ボタン入力時に呼ばれる関数を登録
        pageControllAction.started += ChangePage;

        //ページの最大数を取得
        maxPage = pageDatas.Length - 1;
    }

    private void OnDestroy()
    {
        //ボタン検知を無効化
        pageControllAction.Disable();
        //ボタン入力時に呼ばれる関数を解除
        pageControllAction.started -= ChangePage;
    }

    private void Start()
    {
        //最初のページを表示
        SetSignboardGUI();
    }

    /// <summary>
    /// ページ切り替え入力
    /// </summary>
    /// <param name="context"></param>
    void ChangePage(InputAction.CallbackContext context)
    {
        bool isNext = context.ReadValue<float>() > 0;
        if (isNext)
        {
            //今が最後のページなら無視
            if (nowPage == maxPage) return;
            nowPage++;
        }
        else
        {
            //今が最初のページなら無視
            if (nowPage == 0) return;
            nowPage--;
        }
        //ページの更新
        SetSignboardGUI();
    }

    /// <summary>
    /// GUI表示内容の切り替え
    /// </summary>
    void SetSignboardGUI()
    {
        //アイテムの紹介
        if (pageDatas[nowPage].isItem)
        {
            title.text = pageDatas[nowPage].title;
            itemName.text = pageDatas[nowPage].itemName;
            itemInfo.text = pageDatas[nowPage].info;
            itemIcon.sprite = pageDatas[nowPage].itemIcon;
        }
        //その他
        else
        {
            title.text = pageDatas[nowPage].title;
            info.text = pageDatas[nowPage].info;
        }
        //動画を挿入
        video.clip = pageDatas[nowPage].movie;
        //各パネルの切り替え
        infoPanel.SetActive(!pageDatas[nowPage].isItem);
        itemPanel.SetActive(pageDatas[nowPage].isItem);
        //動画再生
        video.Play();
    }
}
