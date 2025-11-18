using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class PlayerUIButton : MonoBehaviour, ISubmitHandler, ISelectHandler, IDeselectHandler
{
    [SerializeField,Header("各カーソルImage")] RectTransform[] selectImageTf = new RectTransform[4];
    RectTransform myTf;

    private void Awake()
    {
        myTf = gameObject.GetComponent<RectTransform>();
    }

    /// <summary>
    /// ボタンが選択から離れた時に呼ばれる
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDeselect(BaseEventData eventData)
    {
        //PlayerIndexを取得
        int playerIndex = GetPlayerIndex();
        if (playerIndex == -1) return;
    }

    /// <summary>
    /// ボタンが選択された時に呼ばれる
    /// </summary>
    /// <param name="eventData"></param>
    public void OnSelect(BaseEventData eventData)
    {
        //PlayerIndexを取得
        int playerIndex = GetPlayerIndex();
        if (playerIndex == -1) return;
        //PlayerIndexによってカーソルを表示する
        Highlight(playerIndex);
    }

    /// <summary>
    /// 決定された時
    /// </summary>
    /// <param name="eventData"></param>
    public void OnSubmit(BaseEventData eventData)
    {
        //PlayerIndexを取得
        int playerIndex = GetPlayerIndex();

        //---ここからスキル選択処理(まだ)
    }

    /// <summary>
    /// 最後に操作したプレイヤーのPlayerInput.Indexを取得
    /// </summary>
    /// <returns></returns>
    int GetPlayerIndex()
    {
        //最後に操作したEventSystemを探す
        EventSystem mes = MultiplayerEventSystem.current;
        if(mes == null) return -1;
        //EventSystemからPlayerInputを取得
        PlayerInput player = mes.GetComponent<PlayerInput>();
        if (player == null) return -1;
        //PlayerInputのIndexを返却
        return player.playerIndex;
    }

    /// <summary>
    /// 渡されたプレイヤー番号のカーソルを表示＆非表示
    /// </summary>
    /// <param name="playerIndex"></param>
    /// <param name="active"></param>
    void Highlight(int playerIndex)
    {
        selectImageTf[playerIndex].position = myTf.position;
    }
}
