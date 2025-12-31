using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class IllutionTrapSc : TrapThrow
{
    [SerializeField] GameObject[] dummyCharactorPf;
    CorseCheck corseCheck;
    PlayerManager dummyPm;

    protected override void LateStart()
    {
        Invoke(nameof(TimeUp), effectTime[rankingPower]);
        Invoke(nameof(FinishDammyTrapCoolTime), pm.trap.coolTime);
        //主を幻影出現状態にする
        pm.playerController.EffectisIllution(true, gameObject.name);
    }

    protected override void LandedTrap()
    {
        GameObject managerObj = GameObject.FindWithTag("GameController");
        if (managerObj.TryGetComponent(out corseCheck)) { }
        else Debug.LogError("コースチェックが見つかりません");

        //コントローラーを付与してダミーを生成
        PlayerInput player = PlayerInput.Instantiate(
            prefab: dummyCharactorPf[pm.playerNum],
            pairWithDevice: pm.playerController.GetComponent<PlayerInput>().devices[0]
        );
        //この場所に移動
        player.transform.position = transform.position;
        player.transform.rotation = transform.rotation;
        //コースチェックを渡す
        dummyPm = player.transform.parent.GetComponent<PlayerManager>();
        dummyPm.corseCheck = corseCheck;
        //トラップの登録
        dummyPm.playerController.trapObj = pm.playerController.trapObj;
        dummyPm.trap.trapNum = pm.trap.trapNum;
        //トラップアイコンの登録
        dummyPm.iconManager.GetIconSprite();
        //トラップを一時的に禁止
        dummyPm.trap.BunTrap(true);
        //ランキングを主から参照して代入
        dummyPm.playerData.ranking = pm.playerData.ranking;
        //ダミーは狐のお面を使用できない
        dummyPm.playerController.isIllution = true;
        //開始関数を起動
        dummyPm.playerController.StartRace();

        //ダミーのコントローラーにこのアイテムを登録
        dummyPm.playerController.transform.GetComponent<DummyPlayerController>().illutionTrap = this;
    } 

    /// <summary>
    /// ダミーのアイテムクールタイム終了
    /// </summary>
    void FinishDammyTrapCoolTime()
    {
        //トラップの使用を解除
        dummyPm.trap.BunTrap(false);
    }

    /// <summary>
    /// トラップを破壊する
    /// </summary>
    public void TimeUp()
    {
        //まだダミーが破壊されていない場合破壊
        if(dummyPm.gameObject != null)
        {
            //ダミー破壊
            Destroy(dummyPm.gameObject);
            //主に幻影消滅を通達
            pm.playerController.EffectisIllution(false, gameObject.name);
        }
        //罠自体を破壊
        Destroy(gameObject);
    }
}
