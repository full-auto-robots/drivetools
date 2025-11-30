using TMPro;
using UnityEngine;

[RequireComponent(typeof(uim_generic))]
public class uim_vectorfield : MonoBehaviour
{
    public TMP_InputField in_x;
    public TMP_InputField in_y;
    public TMP_InputField in_z;

    void Awake()
    {
        GetComponent<uim_generic>().pushData = () => Initialize();
    }

    public void Initialize()
    {

    }
}
