using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class dsp_layout
{
    public List<dsp_genericdata> displays;
    public net_ntdatalist networkTables;
    public float toolBarPositionX;
    public float toolBarPositionY;
    public ushort toolbarMode;
    public List<byte[]> fieldMarkers;
}
