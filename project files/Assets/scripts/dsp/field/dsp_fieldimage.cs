using UnityEngine;

[System.Serializable]
public class dsp_fieldimage
{
    public Texture2D img;

    public Vector2 minAreaPoint;
    public Vector2 maxAreaPoint;

    public dsp_fieldimage() {}

    public dsp_fieldimage(Texture2D img, Vector2 minAreaPoint, Vector2 maxAreaPoint)
    {
        this.img = img;
        this.minAreaPoint = minAreaPoint;
        this.maxAreaPoint = maxAreaPoint;
    }
}
