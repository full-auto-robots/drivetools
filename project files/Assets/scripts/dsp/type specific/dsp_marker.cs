using System;
using UnityEngine.UI;
using UnityEngine;

public class dsp_marker : MonoBehaviour
{
    public nav_marker data;
    // meters
    public double bumperThickness; // since this doesn't change during a match and is only UI, its not in the data class

    public double metersToPixelsConversionFactor;
    public RectTransform r_fieldImg;

    public RectTransform rt_field;

    // TODO: maybe not in update?
    void Update()
    {
        Draw();
    }

    public void Draw()
    {
        float fieldZAngle = rt_field.GetChild(0).eulerAngles.z;
        RectTransform rect = rt_field.GetChild(0).GetChild(0).GetComponent<RectTransform>();

        // converting field position in meters to pixels
        transform.position = rect.transform.position + Sys.AdjustVectorXY(new Vector3(rect.sizeDelta.x / 2f, rect.sizeDelta.y / 2f, 0) * -1
        + new Vector3((float)(data.position.x * metersToPixelsConversionFactor), (float)(data.position.y * metersToPixelsConversionFactor), 0), fieldZAngle);
        // rotation is easier

        transform.rotation = Quaternion.Euler(new Vector3(0, 0, (float)data.zAngle + fieldZAngle));

        GetComponent<RawImage>().color = data.color;

        GetComponent<RectTransform>().sizeDelta = Vector2.one * (float)data.size * (float)metersToPixelsConversionFactor;

        // make sure the index is valid
        if (data.displayIndex < MarkerMenu.Instance.currentFieldMarkers.Length && data.displayIndex >= 0)
        {
            GetComponent<RawImage>().texture = MarkerMenu.Instance.currentFieldMarkers[data.displayIndex];
        }
    }
}
