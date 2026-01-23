using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class IllutionTrapSc : TrapThrow
{
    [SerializeField,Header("ダミーキャラリスト")] GameObject[] dummyCharactorPf;
    [SerializeField, Header("消える為にかかる時間")] float timeItTakesToClear = 0.2f;
    [SerializeField, Header("効果音")] AudioSource audioSource;
    CorseCheck corseCheck;
    RaceManager race;
    BattleManager battle;
    TutorialManager tutorial;
    PlayerManager.GameMode gameMode;
    PlayerManager dummyPm;

    protected override void LateStart()
    {
        Invoke(nameof(TimeUp), effectTime[rankingPower]);
        Invoke(nameof(FinishDammyTrapCoolTime), pm.trap.coolTime);
        //主を幻影出現状態にする
        pm.playerController.EffectisIllution(true, gameObject.name);
        //けむりをゆっくり消す
        StartCoroutine(FadeAway(timeItTakesToClear));
    }

    protected override void LandedTrap()
    {
        GameObject managerObj = GameObject.FindWithTag("GameController");
        //コースチェックを取得
        if (managerObj.TryGetComponent(out corseCheck)) { }
        else Debug.LogError("コースチェックが見つかりません");
        //各マネージャーのいずれかを取得
        if (managerObj.TryGetComponent(out race)) { gameMode = PlayerManager.GameMode.Race; }
        else if (managerObj.TryGetComponent(out battle)) { gameMode = PlayerManager.GameMode.Battle; }
        else if (managerObj.TryGetComponent(out tutorial)) { gameMode = PlayerManager.GameMode.Tutorial; }
        else Debug.LogError("マネージャーが見つかりません");

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

        if(gameMode == PlayerManager.GameMode.Race)
        {
            race.AddOrRemoveDummyPlayerObj(dummyPm.gameObject, isAdd: true);
        }
        else if (gameMode == PlayerManager.GameMode.Battle)
        {
            battle.AddOrRemoveDummyPlayerObj(dummyPm.gameObject, isAdd: true);
        }

        //SEを再生
        audioSource.Play();
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
    /// ゆっくり消える
    /// </summary>
    IEnumerator FadeAway(float speedTime)
    {
        yield return new WaitForSeconds(0.5f);
        SpriteRenderer sr = trapObj.GetComponent<SpriteRenderer>();
        Color32 color = sr.color;

        WaitForSeconds wait = new(speedTime / 20);  //１ループで待つ時間
        byte reducAlpha = (byte)(color.a / 20);     //１ループで消える割合

        //だんだん透明に
        for (int i = 0; i < 20; i++)
        {
            yield return wait;
            color.a -= reducAlpha;
            sr.color = color;
        }
        //スプライト自体を非表示
        sr.enabled = false;
    }

    /// <summary>
    /// トラップを破壊する
    /// </summary>
    public void TimeUp()
    {
        //マネージャーのダミーリストから削除
        if (gameMode == PlayerManager.GameMode.Race)
        {
            race.AddOrRemoveDummyPlayerObj(dummyPm.gameObject, isAdd: false);
        }
        else if (gameMode == PlayerManager.GameMode.Battle)
        {
            battle.AddOrRemoveDummyPlayerObj(dummyPm.gameObject, isAdd: false);
        }

        //まだダミーが破壊されていない場合破壊
        if (dummyPm.gameObject != null)
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
