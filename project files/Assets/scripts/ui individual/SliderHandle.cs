using UnityEngine;

public class SliderHandle : MonoBehaviour
{
    public bool isGrabbed;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && CanvasUtils.IsCursorInteract(gameObject, true))
        {
            isGrabbed = true;
        }
        if (!Input.GetMouseButton(0))
        {
            isGrabbed = false;
        }

        if (isGrabbed)
        {
            Vector3 offsetVector = Vector3.Project(Input.mousePosition - transform.parent.position, transform.parent.right);
            if (Vector3.Dot(transform.parent.right, offsetVector) > 0)
            {
                transform.position = transform.parent.position + offsetVector.normalized * Mathf.Min(offsetVector.magnitude, transform.parent.GetComponent<RectTransform>().sizeDelta.x);
            }

        }
    }

    public float GetValue()
    {
        Vector3 offsetVector = Vector3.Project(transform.position - transform.parent.position, transform.parent.right);
        return offsetVector.magnitude / transform.parent.GetComponent<RectTransform>().sizeDelta.x;
    }
}
