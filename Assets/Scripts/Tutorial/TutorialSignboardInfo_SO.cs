using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(menuName ="SOs/TutorialSignboardSO")]
public class TutorialSignboardInfo_SO : ScriptableObject
{
    [Tooltip("タイトル")] public string title;
    [Tooltip("説明文"),TextArea] public string info;
    [Header("アイテムかどうか")] public bool isItem;
    [Tooltip("アイテム名")] public string itemName;
    [Tooltip("アイテム名")] public Sprite itemIcon;
    [Header("動画")] public VideoClip movie;
}
