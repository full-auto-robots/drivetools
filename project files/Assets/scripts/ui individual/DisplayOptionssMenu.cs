using UnityEngine;
using UnityEngine.UI;

public class DisplayOptionssMenu : MonoBehaviour
{
    public GameObject g_bounds;
    public Transform t_bg;

    public Button fullscreenButton;
    public Button deleteButton;
    public Button editButton;
    public Button fieldEditorButton;

    public bool isActive;

    void Start()
    {
        gameObject.SetActive(false);
    }

    // assigning the onClick actions to the three buttons
    public void Load(dsp_mainvisual obj)
    {
        float totalHeight = 0;
        for (int i = 0; i < t_bg.childCount; i++)
        {
            if (t_bg.GetChild(i).gameObject.activeSelf)
            {
                totalHeight += t_bg.GetChild(i).GetComponent<RectTransform>().sizeDelta.y;
            }
        }
        t_bg.GetComponent<RectTransform>().sizeDelta = new Vector2(
            t_bg.GetComponent<RectTransform>().sizeDelta.x, totalHeight
        );

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

        if (obj.transform.GetChild(9).GetComponent<dsp_field>() != null)
        {
            fieldEditorButton.gameObject.SetActive(true);

            fieldEditorButton.onClick.RemoveAllListeners();
            fieldEditorButton.onClick.AddListener(() =>
            {
                obj.transform.GetChild(9).GetComponent<dsp_field>().ToggleEditMode();
            });
        } else
        {
            fieldEditorButton.gameObject.SetActive(false);
        }
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
