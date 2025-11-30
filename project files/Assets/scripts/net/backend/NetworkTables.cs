using UnityEngine;

public class NetworkTables
{
    public void Initialize()
    {
        Debug.Log(NtCoreInterop.NT_CreateInstance());
    }
}