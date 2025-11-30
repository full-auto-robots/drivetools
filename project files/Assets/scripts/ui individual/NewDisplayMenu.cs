using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NewDisplayMenu : MonoBehaviour
{
    public GameObject p_keyEntry;
    public float entrySpacing;
    public Transform entryContainer;
    public RectTransform rt_bg;
    public RectTransform rt_grabbyBit;

    public bool[] doUseDisplayTypes;
    private int useableDisplayCount;
    public string[] displayTypes;
    public Sprite[] displayIcons;
    public DisplayType[] displayIndices;

    void Start()
    {
        transform.GetChild(0).gameObject.SetActive(false);

        useableDisplayCount = 0;
        for (int i = 0; i < doUseDisplayTypes.Length; i++)
        {
            if (doUseDisplayTypes[i]) useableDisplayCount++;
        }
    }

    // may want to incorporate this into a GenericMenu class later, and have a UnityAction that calls Show() and Hide()
    public void Refresh()
    {
        Vector2 keySizeDelta = p_keyEntry.GetComponent<RectTransform>().sizeDelta;
        rt_bg.sizeDelta = new Vector2(keySizeDelta.x, keySizeDelta.y * useableDisplayCount);
        rt_grabbyBit.sizeDelta = new Vector2(keySizeDelta.x, rt_grabbyBit.sizeDelta.y);

        CanvasUtils.DestroyChildren(entryContainer.gameObject);

        float existingSpacing = 0;
        for (int i = 0; i < displayTypes.Length; i++)
        {
            if (!doUseDisplayTypes[i]) continue;

            // may as well pass .zero because we have to set the parent and do all that
            Transform t_newKeyEntry = Instantiate(p_keyEntry, Vector3.zero, Quaternion.identity).transform;

            t_newKeyEntry.SetParent(entryContainer);
            t_newKeyEntry.localPosition = -Vector3.up * existingSpacing;
            t_newKeyEntry.GetChild(0).GetComponent<TextMeshProUGUI>().text = displayTypes[i];
            t_newKeyEntry.GetChild(1).GetComponent<Image>().sprite = displayIcons[i];

            existingSpacing += entrySpacing + t_newKeyEntry.GetComponent<RectTransform>().sizeDelta.y;

            int j = i;
            // the entry prefabs function as buttons, that when pressed will spawn a display for that specific variable
            t_newKeyEntry.GetComponent<Button>().onClick.AddListener(() => DisplayManager.Instance.CreateNewDisplayObject((ushort)displayIndices[j]));
        }
    }
}
