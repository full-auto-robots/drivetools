using UnityEngine;

// editor script to attach to floating menu objects

[ExecuteInEditMode]
public class ui_menudebugger : MonoBehaviour
{
    [Header("CONSOLE")]
    public bool showMenu;
    public bool hideMenu;

    void Update()
    {
        if (showMenu)
        {
            showMenu = false;
            transform.GetChild(0).gameObject.SetActive(true);
        }
        if (hideMenu)
        {
            hideMenu = false;
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }
}
