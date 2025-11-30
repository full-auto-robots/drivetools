using TMPro;
using UnityEngine;

[RequireComponent(typeof(uim_generic))]
public class uim_inputfield : MonoBehaviour
{
    public TMP_InputField in_data;

    void Awake()
    {
        GetComponent<uim_generic>().pushData = () => { Initialize(); };
    }

    public void Initialize()
    {
        //Debug.Log(22222 + GetComponent<uim_generic>().currentData);
        in_data.text = GetComponent<uim_generic>().currentData;
        in_data.onEndEdit.RemoveAllListeners();

        // weird notation but IT WORKS so im not complaining lmao
        in_data.onEndEdit.AddListener((x) => GetComponent<uim_generic>().onUpdate.Invoke(x));
    }
    
    // void Update()
    // {
    //     Debug.Log(22222 + GetComponent<uim_generic>().currentData);
    // }
}
