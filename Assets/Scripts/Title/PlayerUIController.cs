using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUIController : MonoBehaviour
{
    TitleManager titleManager;
    PlayerInput playerInput;

    private void Awake()
    {
        //タグを頼りにタイトルマネージャーを取得
        titleManager = GameObject.FindWithTag("GameController").GetComponent<TitleManager>();
        //コンポーネント取得
        playerInput = GetComponent<PlayerInput>();
    }

    #region 入力

    /// <summary>
    /// 決定ボタン
    /// </summary>
    /// <param name="context"></param>
    public void OnDecision(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            titleManager.Decision(playerInput.playerIndex);
        }
    }

    /// <summary>
    /// キャンセルボタン
    /// </summary>
    /// <param name="context"></param>
    public void OnCancel(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            titleManager.Cancel(playerInput.playerIndex);
        }
    }

    /// <summary>
    /// キャラクター変更ボタン
    /// </summary>
    /// <param name="context"></param>
    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Vector2 vec = context.ReadValue<Vector2>();
            titleManager.OnMove(playerInput.playerIndex, vec);
        }
    }

    public void OnInfo(InputAction.CallbackContext context)
    {

    }

    #endregion

    /// <summary>
    /// デバイスを失ったとき
    /// </summary>
    /// <param name="input"></param>
    public void OnDeviceLostEvent(PlayerInput input)
    {
        //今は仮で破壊
        Destroy(gameObject);
    }
}
