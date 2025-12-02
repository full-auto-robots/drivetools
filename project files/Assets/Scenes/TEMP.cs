using System;
using System.Runtime.InteropServices;
using UnityEngine;

[StructLayout(LayoutKind.Sequential)]
public struct WPI_String
{
    // const char* str - pointer to the UTF8/ANSI string contents
    // We use IntPtr to hold the pointer to the managed string data
    public IntPtr str; 
    
    // size_t len - length of the string, which is platform-dependent (4 or 8 bytes)
    public UIntPtr len; 
}

public class TEMP : MonoBehaviour
{
    private NetworkTables n;
    private UIntPtr i;
    void Start()
    {
        n = new NetworkTables();

        i = NtCoreInterop.NT_GetDefaultInstance();
        Debug.Log(i);

        if (i != (UIntPtr)0)
        {
            n.ConnectToServerV3((UIntPtr)2386, i);
        }
    }

    void Update()
    {
        if (n != null) 
            Debug.Log(n.IsConnectedV3(i));
    }
}
