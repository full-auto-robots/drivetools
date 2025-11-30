using UnityEngine;
using UnityEngine.Events;

// "adapter" between the modular menu and whatever class is running on this object

public class uim_generic : MonoBehaviour
{
    public UnityAction pushData; // called when this scripts data is updated

    // passed onto the specific class, called when the player finishes editing a piece of data
    public UnityAction<string> onUpdate; // has to have a string so we can support data
    public string currentData; // used to populate the comp to show the data

    public float effectiveHeight;
}
