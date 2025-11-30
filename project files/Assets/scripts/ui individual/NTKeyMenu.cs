using TMPro;
using UnityEngine;
using UnityEngine.UI;

// showing a list of keys already available on NT, either the real nt or fake it doesn't matter

public class NTKeyMenu : MonoBehaviour
{
    public GameObject p_keyEntry;
    public float entrySpacing;
    public Transform entryContainer;
    public RectTransform rt_bg;
    public RectTransform rt_grabbyBit;

    void Start()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }

    // may want to incorporate this into a GenericMenu class later, and have a UnityAction that calls Show() and Hide()
    public void Refresh()
    {
        Vector2 keySizeDelta = p_keyEntry.GetComponent<RectTransform>().sizeDelta;
        rt_bg.sizeDelta = new Vector2(keySizeDelta.x, keySizeDelta.y * NetworkManager.Instance.latestData.dataKeys.Length);
        rt_grabbyBit.sizeDelta = new Vector2(keySizeDelta.x, rt_grabbyBit.sizeDelta.y);
        CanvasUtils.DestroyChildren(entryContainer.gameObject);
        float existingSpacing = 0;
        for (int i = 0; i < NetworkManager.Instance.latestData.dataKeys.Length; i++)
        {
            // may as well pass .zero because we have to set the parent and do all that
            Transform t_newKeyEntry = Instantiate(p_keyEntry, Vector3.zero, Quaternion.identity).transform;

            t_newKeyEntry.SetParent(entryContainer);
            t_newKeyEntry.localPosition = -Vector3.up * existingSpacing;
            t_newKeyEntry.GetChild(0).GetComponent<TextMeshProUGUI>().text = NetworkManager.Instance.latestData.dataKeys[i];
            t_newKeyEntry.GetChild(1).GetComponent<Image>().color = Sys.typeColors[Sys.GetTypeIndex(NetworkManager.Instance.latestData.dataValues[i])];

            existingSpacing += entrySpacing + t_newKeyEntry.GetComponent<RectTransform>().sizeDelta.y;

            // we do need to figure out what kind of a class that key holds, so we can color-code different variable types
            ushort requiredDisplayType = Sys.GetDisplayObjectIndex(NetworkManager.Instance.latestData.dataValues[i]);

            // the entry prefabs function as buttons, that when pressed will spawn a display for that specific variable
            t_newKeyEntry.GetComponent<Button>().onClick.AddListener(() => DisplayManager.Instance.CreateNewDisplayObject(requiredDisplayType, NetworkManager.Instance.latestData.dataKeys[i]));
        }
    }
}
