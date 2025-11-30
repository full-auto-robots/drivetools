using UnityEngine;
using UnityEngine.Events;

// works along with ToolbarMenu.cs
public class FloatingMenu : MonoBehaviour
{
    public GameObject g_menuParent;
    public GameObject g_interactPoint;

    public bool isGrabbed;
    public ushort state;

    private Vector3 cursorOffset;

    public UnityAction handlePositioning;
    public UnityAction onGrab;

    void Awake()
    {
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && CanvasUtils.IsCursorInteract(g_interactPoint, false))
        {
            isGrabbed = true;
            if (onGrab != null) onGrab.Invoke();
            state = (ushort)DockingMode.Floating;

            cursorOffset = transform.position - Input.mousePosition;
        }
        if (!Input.GetMouseButton(0))
        {
            isGrabbed = false;
        }

        if (handlePositioning != null) handlePositioning.Invoke();

        if (isGrabbed && state == (ushort)DockingMode.Floating)
        {
            transform.position = Input.mousePosition + cursorOffset;
        }
    }
}
