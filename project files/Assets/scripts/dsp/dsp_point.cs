using UnityEngine.UI;
using UnityEngine;

public class dsp_point : MonoBehaviour
{
    public Image i_comp;
    public bool isGrabbed;
    public dsp_pointhandler parent;
    public Vector3 carryOffset;

    // called from Update()
    public void HandleInteract()
    {
        if (CanvasUtils.IsCursorInteract(gameObject, false))
        {
            if (Input.GetMouseButtonDown(0) && !DisplayManager.Instance.areDisplaysLocked)
            {
                isGrabbed = true;
                carryOffset = Vector3.zero;
                SetParentIndex();
                parent.SetRatio();
            }

            i_comp.color = new Color(1,1,1,1);
        } else
        {
            i_comp.color = new Color(0,0,0,0);
        }
        
        if (isGrabbed)
        {
            parent.lockProportions = Input.GetKey(KeyCode.LeftShift);
            parent.lockPointIndex = int.Parse(gameObject.name);

            transform.position = Input.mousePosition + carryOffset;

            if (!Input.GetMouseButton(0))
            {
                isGrabbed = false;
                parent.lockProportions = false;
                ClearParentIndex();
            }
        }
    }

    public void SetParentIndex()
    {
        if (parent != null) { parent.grabIndex = int.Parse(gameObject.name); DisplayManager.Instance.UpdatePreviousLayout(); }
    }
    public void ClearParentIndex()
    {
        if (parent != null) { parent.grabIndex = -1; }
    }

    public void Initialize()
    {
        i_comp = GetComponent<Image>();
        parent = transform.parent.GetComponent<dsp_pointhandler>();
        GetComponent<Image>().color = Color.red;
        GetComponent<RectTransform>().sizeDelta = Vector2.one * 20;
        if (gameObject.name == "0") { transform.localPosition = Vector3.zero * 80; } // center
        if (gameObject.name == "1") { transform.localPosition = Vector3.left * 80; } // left
        if (gameObject.name == "2") { transform.localPosition = Vector3.right * 80; } // right
        if (gameObject.name == "3") { transform.localPosition = Vector3.up * 80; } // up
        if (gameObject.name == "4") { transform.localPosition = Vector3.down * 80; } // down
        if (gameObject.name == "5") { transform.localPosition = Vector3.left * 80 + Vector3.up * 80; } // top left
        if (gameObject.name == "6") { transform.localPosition = Vector3.right * 80 + Vector3.up * 80; }  // top right
        if (gameObject.name == "7") { transform.localPosition = Vector3.left * 80 + Vector3.down * 80; } // bottom left
        if (gameObject.name == "8") { transform.localPosition = Vector3.right * 80 + Vector3.down * 80; } // bottom right
    }
}
