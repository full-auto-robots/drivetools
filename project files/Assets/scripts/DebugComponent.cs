using UnityEngine.UI;
using UnityEngine;

// oookay so this is either a smart system or an abysmally dumb one

// I need to enable and disable debug objects based on a keystroke, 
// and to avoid the use of many inspector references I'm just giving all debug objects this component
// the UIManager.cs will look for them and turn them on/off
public class DebugComponent : MonoBehaviour
{
    public void Deactivate()
    {
        // easier to set alpha to zero for image components, as opposed to turning them off
        if (GetComponent<Image>() != null)
        {
            Color col = GetComponent<Image>().color;
            GetComponent<Image>().color = new Color(col.r, col.g, col.b, 0);
        }
        else if (GetComponent<RawImage>() != null)
        {
            Color col = GetComponent<RawImage>().color;
            GetComponent<RawImage>().color = new Color(col.r, col.g, col.b, 0);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void Activate()
    {
        // easier to set alpha to zero for image components, as opposed to turning them off
        if (GetComponent<Image>() != null)
        {
            Color col = GetComponent<Image>().color;
            GetComponent<Image>().color = new Color(col.r, col.g, col.b, 0.3f);
        }
        else if (GetComponent<RawImage>() != null)
        {
            Color col = GetComponent<RawImage>().color;
            GetComponent<RawImage>().color = new Color(col.r, col.g, col.b, 0.3f);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }
}
