using UnityEngine;
using UnityEngine.UI;

public class DisplayOptionssMenu : MonoBehaviour
{
    public GameObject g_bounds;

    public Button fullscreenButton;
    public Button deleteButton;
    public Button editButton;

    public bool isActive;

    void Start()
    {
        gameObject.SetActive(false);
    }

    // assigning the onClick actions to the three buttons
    public void Load(dsp_mainvisual obj)
    {
        DisplayManager.Instance.g_displayEditor.SetActive(false);
        DisplayManager.Instance.g_displayEditor.transform.parent.GetComponent<ui_modularmenu>().ClearAllComponents();

        deleteButton.onClick.RemoveAllListeners();
        deleteButton.onClick.AddListener(() => DisplayManager.Instance.DeleteDisplayObject(obj));

        fullscreenButton.onClick.RemoveAllListeners();
        if (obj.GetComponent<dsp_pointhandler>().isFullScreen) {
            fullscreenButton.onClick.AddListener(() => obj.GetComponent<dsp_pointhandler>().ExitFullscreen());
        } else {
            fullscreenButton.onClick.AddListener(() => obj.GetComponent<dsp_pointhandler>().SetAsFullscreen());
        }

        editButton.onClick.RemoveAllListeners();
        editButton.onClick.AddListener(() => {   DisplayManager.Instance.EditDisplay(obj); });
    }

    void Update()
    {
        if (isActive)
        {
            if (!CanvasUtils.IsCursorInBounds(g_bounds, false))
            {
                isActive = false;
                gameObject.SetActive(false);
            }
        }
    }
}
