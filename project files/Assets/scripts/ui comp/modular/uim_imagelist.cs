using UnityEngine;

// this class essentially extends the ui_imagelist one
// I figure that's the best way to do it, because it means there's only 1 set of image list code

// so all this class really serves to do is add uim functionality onto ui_imagelist

[RequireComponent(typeof(uim_generic))]
public class uim_imagelist : MonoBehaviour
{
    public ui_imagelist comp;
    
    // just doing the initialization on awake here
    void Awake()
    {
        
    }
}
