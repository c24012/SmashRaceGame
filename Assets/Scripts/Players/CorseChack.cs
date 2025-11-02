using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorseChack : MonoBehaviour
{
    public enum EAttribute
    {
        None,
        Road,   //道
        Dart,   //ダート
        Out,    //場外
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
        //今のピクセルのRGBが[0]なら道判定
        if(color.r == 0)
        {
            return EAttribute.Road;
        }
        //それ以外はダート判定
        return EAttribute.Dart;
    }
}
