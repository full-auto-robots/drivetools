using TMPro;
using UnityEngine;

public class ui_tabs : MonoBehaviour
{
    // also provides the count
    public string[] tabNames;
    public bool initializeOnStart;

    public GameObject p_tab;
    public Transform t_tabContainer;

    void Start()
    {
        if (initializeOnStart) Initialize();
    }

    public void InitializeFromChildren(Transform parent)
    {
        string[] names = new string[parent.childCount];
        for (int i = 0; i < parent.childCount; i++)
        {
            names[i] = parent.GetChild(i).gameObject.name;
        }

        tabNames = names;
        Initialize();
    }

    public void Initialize()
    {
        float existingSpace = 0;
        for (int i = 0; i < tabNames.Length; i++)
        {
            Transform t_newTab = Instantiate(p_tab, t_tabContainer).transform;
            t_newTab.localPosition = Vector3.right * existingSpace;

            t_newTab.GetComponent<TextMeshProUGUI>().text = tabNames[i];

            existingSpace += t_newTab.GetComponent<RectTransform>().sizeDelta.x;
        }
    }
}
