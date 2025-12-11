using UnityEngine;

// not really a "menu" per se, but I'm using the suffix to refer to one-off scripts that affect UI components so whatever

public class ToolbarMenu : MonoBehaviour
{
    public FloatingMenu parent;


    public RectTransform rt_bg;
    public RectTransform rt_grabbyBit;

    public Transform t_itemContainer;

    // the thickness of the menu
    public float minorWidth;
    private float actualMinorWidth;
    public float halfMinorWidth;

    private float defaultIconSize;

    void Start()
    {
        defaultIconSize = t_itemContainer.GetChild(0).GetComponent<RectTransform>().sizeDelta.x;

        parent.onGrab = () => Undock();
        parent.handlePositioning = () => HandlePositioning();

        ResizeToolbar();
    }

    void ResizeToolbar()
    {
        // make sure we actually HAVE a value here
        if (minorWidth == 0)
        {
            minorWidth = 60;
        }

        actualMinorWidth = minorWidth * Sys.uiScaleMultiplier;
        halfMinorWidth = actualMinorWidth / 2f;

        for (int i = 0; i < t_itemContainer.transform.childCount; i++) {
            t_itemContainer.GetChild(i).GetComponent<RectTransform>().sizeDelta = new Vector2(defaultIconSize * Sys.uiScaleMultiplier, defaultIconSize * Sys.uiScaleMultiplier);
        }
        rt_grabbyBit.sizeDelta = new Vector2(defaultIconSize * Sys.uiScaleMultiplier, defaultIconSize * Sys.uiScaleMultiplier);
    }

    public void Undock()
    {
        DockingManager.Instance.Undock(parent.state, minorWidth);
        parent.state =(ushort) DockingMode.Floating;
    }

    void HandlePositioning()
    {
        ResizeToolbar();
        // oh boy this is a terrible way of doing things
        if (gameObject.name == "toolbar")
        {
            if (NetworkManager.Instance.isInViewingMode)
            {
                t_itemContainer.GetChild(3).gameObject.SetActive(false);
                t_itemContainer.GetChild(4).gameObject.SetActive(false);

                t_itemContainer.GetChild(5).gameObject.SetActive(false);
                t_itemContainer.GetChild(6).gameObject.SetActive(false);
                t_itemContainer.GetChild(7).gameObject.SetActive(false);
                t_itemContainer.GetChild(8).gameObject.SetActive(false);
                t_itemContainer.GetChild(0).gameObject.SetActive(false);
            }
            else
            {
                t_itemContainer.GetChild(3).gameObject.SetActive(false);  // both false bc we cannot access items from NT
                t_itemContainer.GetChild(4).gameObject.SetActive(true);

                t_itemContainer.GetChild(5).gameObject.SetActive(true);
                t_itemContainer.GetChild(6).gameObject.SetActive(true);
                t_itemContainer.GetChild(7).gameObject.SetActive(false);
                t_itemContainer.GetChild(8).gameObject.SetActive(false);
                t_itemContainer.GetChild(0).gameObject.SetActive(false);
            }
        }


        if (parent.state == (ushort)DockingMode.Floating)
        {
            TryDock();
            rt_bg.localPosition = -Vector3.right * halfMinorWidth;
            rt_bg.sizeDelta = new Vector2((defaultIconSize * Sys.uiScaleMultiplier + 15) * t_itemContainer.childCount, actualMinorWidth);
            PopulateToolbar(Vector2.right);
        }
        else if (parent.state == (ushort)DockingMode.DockedUpper)
        {
            rt_bg.sizeDelta = new Vector2(Screen.width, actualMinorWidth);
            rt_bg.localPosition = -Vector3.right * halfMinorWidth;
            transform.position = new Vector3(halfMinorWidth, DockingManager.EffectiveUpper() + minorWidth - halfMinorWidth);
            PopulateToolbar(Vector2.right);
        }
        else if (parent.state == (ushort)DockingMode.DockedLower)
        {
            rt_bg.sizeDelta = new Vector2(Screen.width, actualMinorWidth);
            rt_bg.localPosition = -Vector3.right * halfMinorWidth;
            transform.position = new Vector3(halfMinorWidth, DockingManager.EffectiveLower() - minorWidth + halfMinorWidth);
            PopulateToolbar(Vector2.right);
        }
        else if (parent.state == (ushort)DockingMode.DockedLeft)
        {
            transform.position = new Vector3(DockingManager.EffectiveLeft() - minorWidth + halfMinorWidth, halfMinorWidth);
            rt_bg.sizeDelta = new Vector2(actualMinorWidth, Screen.height);
            rt_bg.localPosition = Vector3.up * (Screen.height / 2f - halfMinorWidth) - Vector3.right * halfMinorWidth;
            PopulateToolbar(Vector2.up);
        }
        else if (parent.state == (ushort)DockingMode.DockedRight)
        {
            transform.position = new Vector3(DockingManager.EffectiveRight() + minorWidth - halfMinorWidth, halfMinorWidth);
            rt_bg.localPosition = Vector3.up * (Screen.height / 2f - halfMinorWidth) - Vector3.right * halfMinorWidth;
            rt_bg.sizeDelta = new Vector2(actualMinorWidth, Screen.height);
            PopulateToolbar(Vector2.up);
        }
    }

    void TryDock()
    {
        if (transform.position.y > Screen.height - 20)
        {
            

            Undock();


            parent.state = (ushort)DockingMode.DockedUpper;
           
            DockingManager.Instance.Dock(parent.state, minorWidth);
        }
        else if (transform.position.y < 20)
        {

            Undock();

            parent.state = (ushort)DockingMode.DockedLower;

            
            DockingManager.Instance.Dock(parent.state, minorWidth);
        }
        else if (transform.position.x > Screen.width - 20)
        {
            

            
            Undock();

            parent.state = (ushort)DockingMode.DockedRight;
            
            DockingManager.Instance.Dock(parent.state, minorWidth);
        }
        else if (transform.position.x < 20)
        {
            

            
            Undock();

            parent.state = (ushort)DockingMode.DockedLeft;
            
            DockingManager.Instance.Dock(parent.state, minorWidth);
        }
    }

    void PopulateToolbar(Vector2 direction)
    {
        float existingSize = 0;
        for (int i = 0; i < t_itemContainer.childCount; i++)
        {
            if (!t_itemContainer.GetChild(i).gameObject.activeSelf) {continue;}
            if (i > 0) { existingSize += t_itemContainer.GetChild(i - 1).GetComponent<RectTransform>().sizeDelta.x + 15; }
            float multiplier = 80 + existingSize;

            t_itemContainer.GetChild(i).position = transform.position + new Vector3(direction.x, direction.y, 0) * multiplier;
        }
    }
}
