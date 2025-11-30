using UnityEngine;

[RequireComponent(typeof(uim_generic))]
public class uim_colorpicker : MonoBehaviour
{
    public ui_colorpicker pickerComp;

    void Awake()
    {
        GetComponent<uim_generic>().pushData = () => Initialize();
    }

    public void Initialize()
    {

    }
}
