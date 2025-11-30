using UnityEngine;

// static storage for the different components, that's all

// does this project have too many scripts?

// anyways I can't do this on ui_modularmenu else I'd have to add the references every time
public class uim : MonoBehaviour // short name so that it can be referenced quickly
{
    private void Awake()
    {
        text = p_text;
        inputField = p_inputField;
        vectorField = p_vectorField;
        imageList = p_imageList;
    }

    // technically these are ALL PREFABS, I'm just only using the prefix on the inspector ones
    // the static ones don't have any prefix so they're less verbose
    public GameObject p_text;
    public static GameObject text;
    public GameObject p_inputField;
    public static GameObject inputField;
    public GameObject p_vectorField;
    public static GameObject vectorField;
    public GameObject p_imageList;
    public static GameObject imageList;
}
