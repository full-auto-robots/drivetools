using TMPro;
using UnityEngine;

[RequireComponent(typeof(uim_generic))]
public class uim_text : MonoBehaviour
{
    public TextMeshProUGUI textDisplay;

    void Awake()
    {
        GetComponent<uim_generic>().pushData = () => Initialize();
    }

    public void Initialize()
    {
        textDisplay.text = GetComponent<uim_generic>().currentData;
    }
}
