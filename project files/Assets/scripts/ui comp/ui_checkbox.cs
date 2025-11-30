using UnityEngine;
using UnityEngine.Events;

public class ui_checkbox : MonoBehaviour
{
    public bool value;
    public GameObject g_checkmark;
    public UnityEvent onClick;

    void Update()
    {
        if (CanvasUtils.IsCursorInteract(gameObject, true))
        {
            if (Input.GetMouseButtonDown(0))
            {

                value = !value;
                g_checkmark.SetActive(!g_checkmark.activeSelf);

                onClick.Invoke();
            }
        }
    }

    public void Set(bool value)
    {
        this.value = value;
        g_checkmark.SetActive(value);
    }
}
