using System;
using UnityEngine.UI;
using UnityEngine;

public class dsp_robot : MonoBehaviour
{
    public nav_robot data;
    // meters
    public double bumperThickness; // since this doesn't change during a match and is only UI, its not in the data class

    public double metersToPixelsConversionFactor;
    public RectTransform r_fieldImg;

    public RectTransform rt_field;

    public void Draw()
    {
        float fieldZAngle = rt_field.GetChild(0).eulerAngles.z;
        RectTransform rect = rt_field.GetChild(0).GetChild(0).GetComponent<RectTransform>();

        // converting field position in meters to pixels
        transform.position = rect.transform.position + Sys.AdjustVectorXY(new Vector3(rect.sizeDelta.x / 2f, rect.sizeDelta.y / 2f, 0) * -1
        + new Vector3((float)(data.position.x * metersToPixelsConversionFactor), (float)(data.position.y * metersToPixelsConversionFactor), 0), fieldZAngle);
        // rotation is easier
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, (float)data.zAngle + fieldZAngle));

        if (!Sys.IsPointInBounds(rt_field.GetComponent<RectTransform>(), transform.position))
        {
            for (int i = 0; i < 5; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        else
        {
            for (int i = 0; i < 5; i++)
            {
                RectTransform component = transform.GetChild(i).GetComponent<RectTransform>();
                component.gameObject.SetActive(true);
                component.GetComponent<Image>().color = data.color;

                if (i == 4)
                {
                    component.sizeDelta = new Vector2(10, 10);
                }
                else
                {
                    component.sizeDelta = new Vector2((float)(data.width * metersToPixelsConversionFactor), (float)(bumperThickness * metersToPixelsConversionFactor));
                }
                // component.GetComponent<Image>().color = robotColor;

                if (i == 0)
                {
                    component.localPosition = new Vector3(0, (float)((data.width / 2 - bumperThickness / 2) * metersToPixelsConversionFactor), 0);
                    component.localRotation = Quaternion.Euler(0, 0, 0);
                }
                else if (i == 1)
                {
                    component.localPosition = new Vector3(0, (float)((-data.width / 2 + bumperThickness / 2) * metersToPixelsConversionFactor), 0);
                    component.localRotation = Quaternion.Euler(0, 0, 0);
                }
                else if (i == 2)
                {
                    component.localPosition = new Vector3((float)((data.width / 2 - bumperThickness / 2) * metersToPixelsConversionFactor), 0, 0);
                    component.localRotation = Quaternion.Euler(0, 0, 90);
                }
                else if (i == 3)
                {
                    component.localPosition = new Vector3((float)((-data.width / 2 + bumperThickness / 2) * metersToPixelsConversionFactor), 0, 0);
                    component.localRotation = Quaternion.Euler(0, 0, 90);
                }
                else if (i == 4)
                {
                    component.localPosition = new Vector3((float)((data.width / 2 - bumperThickness / 2) * metersToPixelsConversionFactor), 0, 0);
                    component.localRotation = Quaternion.Euler(0, 0, 0);
                }
            }
        }
    }
}
