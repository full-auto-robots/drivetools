using UnityEngine;
using UnityEngine.Events;

public class ui_modularmenu : MonoBehaviour
{
    public Transform t_compContainer;
    public float spacingBetweenMenus;
    public float heightTakenUp;

    /* adding components to the menu
    */

    // this is the "raw" function, everything else should call this
    // (returns the initialized object, in case we need that, so we don't have to call FindComp())
    public GameObject AddComp(GameObject comp, string name)
    {
        GameObject g_newComp = Instantiate(comp, t_compContainer);
        g_newComp.name = name;

        // position the object
        g_newComp.transform.localPosition = -Vector3.up * heightTakenUp;

        heightTakenUp += comp.GetComponent<uim_generic>().effectiveHeight + spacingBetweenMenus;

        return g_newComp;
    }
    // (returns the initialized object, in case we need that, so we don't have to call FindComp())
    public GameObject AddCompWithTitle(GameObject comp, string name)
    {
        // title comes first
        AddComp(uim.text, "title_" + name);
        t_compContainer.GetChild(t_compContainer.childCount - 1).GetComponent<uim_generic>().currentData = name;
        t_compContainer.GetChild(t_compContainer.childCount - 1).GetComponent<uim_generic>().pushData.Invoke();
        
        return AddComp(comp, name);
    }
    /**/

    public void ClearAllComponents()
    {
        for (int i = t_compContainer.childCount - 1; i >= 0; i--)
        {
            // no list of comps to keep track of, so I can just destroy them
            Destroy(t_compContainer.GetChild(i).gameObject);
        }
        heightTakenUp = 0;
    }

    // reloading everything, in case the spacing changes
    public void RefreshALlComponents()
    {
        heightTakenUp = 0;

        for (int i = 0; i < t_compContainer.childCount; i++)
        {
            t_compContainer.GetChild(i).localPosition = -Vector3.up * heightTakenUp;
            heightTakenUp += t_compContainer.GetChild(i).GetComponent<uim_generic>().effectiveHeight
             + spacingBetweenMenus;
        }
    }

    public uim_generic FindComp(string compName)
    {
        for (int i = 0; i < t_compContainer.childCount; i++)
        {
            if (t_compContainer.GetChild(i).gameObject.name == compName)
            {
                return t_compContainer.GetChild(i).GetComponent<uim_generic>();
            }
        }

        return null;
    }

    // the action to do when editing
    public void SetAction(string name, UnityAction<string> action)
    {
        uim_generic comp = FindComp(name);
        comp.onUpdate = action;
        comp.pushData.Invoke();
    }
    // showing the current piece of data
    public void Populate(string name, string data)
    {
        uim_generic comp = FindComp(name);
        comp.currentData = data;
        comp.pushData.Invoke();
    }
}
