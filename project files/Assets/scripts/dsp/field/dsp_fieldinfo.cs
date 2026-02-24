using System.Collections.Generic;
using UnityEngine;

// to reduce verbosity and the number of massive files there are,
// I've started trying to split more things off into their own scripts
// (after all, drivetools is getting quite large)

public class dsp_fieldinfo : MonoBehaviour
{
    public dsp_fieldimage[] includedFieldImages;

    public List<dsp_fieldimage> addedFieldImages; // added by the user
}
