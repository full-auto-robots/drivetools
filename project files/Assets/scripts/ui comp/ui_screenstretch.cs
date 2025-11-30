using UnityEngine;

public class ui_screenstretch : MonoBehaviour
{
    [Header("CONFIG")]
    public bool updatePeriodically;
    public RectTransform stretchTo;

    void Awake()
    {
        Stretch();
    }

    void Stretch()
    {
        if (stretchTo != null)
        {
            GetComponent<RectTransform>().sizeDelta = stretchTo.sizeDelta;
        }
        else
        {
            GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, Screen.height);
        }
    }

    void Update()
    {
        if (updatePeriodically)
        {
            Stretch();
        }
    }
}
