using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class net_ntdatalist
{
    public string[] dataKeys;
    public string[] dataValues;

    public string GetValueAt(string key)
    {
        if (dataKeys == null || dataValues == null) { return ""; }
        int index = -1;
        for (int i = 0; i < dataKeys.Length; i++)
        {
            if (dataKeys[i] == key) index = i;
        }

        if (index != -1)
        {
            return dataValues[index];
        }
        else
        {
            return "";
        }
    }
}
