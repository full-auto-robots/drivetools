using TMPro;
using UnityEngine;

// this menu leverages the ui_modularmenu component a lot
public class DisplayEditingMenu : MonoBehaviour
{
    // no direct references to input fields, bc that's what ui_modularmenu is for

    public dsp_mainvisual loadedDisplay;
    private ui_modularmenu menu;

    void Awake()
    {
        menu = GetComponent<ui_modularmenu>();
    }

    public void Load(dsp_mainvisual obj)
    {
        loadedDisplay = obj; // saved so changes can be updated
        menu.ClearAllComponents();

        menu.AddCompWithTitle(uim.inputField, "nt_key");
        menu.AddCompWithTitle(uim.inputField, "nt_value");

        menu.Populate("nt_key", obj.ntKey);
        if (!obj.isLocked)
        {
            menu.Populate("nt_value", NetworkManager.Instance.latestData.GetValueAt(obj.ntKey));
        }
        else
        {
            menu.Populate("nt_value", loadedDisplay.transform.GetChild(9).GetComponent<TextMeshProUGUI>().text);
        }

        menu.SetAction("nt_key", (x) => { loadedDisplay.ntKey = x; });
        menu.SetAction("nt_value", (x) =>
        {
            if (loadedDisplay.transform.GetChild(9).gameObject.name == "text component")
            {
                loadedDisplay.isLocked = true;
                loadedDisplay.transform.GetChild(9).GetComponent<TextMeshProUGUI>().text = x;
            }
        });

        if (loadedDisplay.customEditingComps != null)
        {
            // now for the custom stuff
            // assuming all 3 of these lists have same count
            for (int i = 0; i < loadedDisplay.customEditingComps.Count; i++)
            {
                // just doing titles by default
                loadedDisplay.onceInitialize[i].Invoke(menu.AddCompWithTitle(loadedDisplay.customEditingComps[i], loadedDisplay.customEditingNames[i]));

                // the data population is done by itself
                // ditto with the action
            }
        }
    }
}
