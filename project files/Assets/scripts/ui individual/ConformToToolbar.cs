using UnityEngine;

public class ConformToToolbar : MonoBehaviour
{
    public ToolbarMenu parentToolbar;

    void Update()
    {
        if (parentToolbar.parent.state == 4 || parentToolbar.parent.state == 2)
        {
            GetComponent<RectTransform>().eulerAngles = new Vector3(0, 0, 90);
        }
        else
        {
            GetComponent<RectTransform>().eulerAngles = new Vector3(0, 0, 0);
        }
    }
}
