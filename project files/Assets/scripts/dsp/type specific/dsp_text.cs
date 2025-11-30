using TMPro;
using UnityEngine;

public class dsp_text : MonoBehaviour
{
    public string ntKey;
    public TextMeshProUGUI textComponent;

    void Awake()
    {
        GetComponent<dsp_mainvisual>().updateAction = (x) => UpdateData(x);
    }

    void Update()
    {
        // TODO: NOT DO THIS IN Update()!
        ntKey = GetComponent<dsp_mainvisual>().ntKey;

        textComponent.GetComponent<RectTransform>().sizeDelta = GetComponent<RectTransform>().sizeDelta;
        textComponent.GetComponent<RectTransform>().localPosition = Vector3.zero;
    }

    void UpdateData(net_ntdatalist data)
    {
        if (GetComponent<dsp_mainvisual>().isLocked) { return; }
        for (int i = 0; i < data.dataKeys.Length; i++)
        {
            if (data.dataKeys[i] == ntKey)
            {
                // set the text component
                textComponent.text = Sys.ParseData(data.dataValues[i]);
            }
        }
    }
}
