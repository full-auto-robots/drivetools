using UnityEngine;

// SHOULD only apply to the unity editor, not a build

[ExecuteAlways]
public class ui_debugger : MonoBehaviour
{
    public UIManager uiManager; // because this is an editor script we can't use the .Instance notation
    [Header("CONSOLE")]
    public bool showAllMenus;
    public bool hideAllMenus;

    void Update()
    {
        if (showAllMenus)
        {
            showAllMenus = false;
            uiManager.SetAllMenusActive(true);
        }
        if (hideAllMenus)
        {
            hideAllMenus = false;
            uiManager.SetAllMenusActive(false);
        }
    }
}
