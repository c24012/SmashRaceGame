using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorseCheck : MonoBehaviour
{
    public enum EAttribute
    {
        None,
        Road,       //道
        RoughRoad,  //荒道
        Dart,       //ダート
        Warning,    //警告
        Out,        //場外
    }

    [SerializeField] Texture2D attributeTexture;
    [SerializeField] int pixcelParUnit;

    /// <summary>
    /// 道の属性を取得
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public EAttribute GetAttribute(Vector3 pos)
    {
        //今いる場所から地図のピクセル座標を取得
        int pixelX = Mathf.Clamp((int)(attributeTexture.width / 2 + pos.x * pixcelParUnit),0, attributeTexture.width);
        int pixelY = Mathf.Clamp((int)(attributeTexture.height / 2 + pos.y * pixcelParUnit),0, attributeTexture.height);

        //今いるピクセルの色を取得
        Color color = attributeTexture.GetPixel(pixelX, pixelY);
        //ダート
        if (color.r == 0) return EAttribute.Dart;
        //荒道
        else if (color.r == 50 / 255f) return EAttribute.RoughRoad;
        //警告
        else if (color.r == 100 / 255f) return EAttribute.Warning;
        //場外
        else if (color.r == 150 / 255f) return EAttribute.Out;
        //通常道
        else if (color.r == 1) return EAttribute.Road;
        Debug.LogError("指定外の道の属性です");
        return EAttribute.None;
    }
}
