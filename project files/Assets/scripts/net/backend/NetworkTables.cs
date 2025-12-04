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

public class NetworkTables
{
    public static UIntPtr defaultNT3Port = (UIntPtr)1735;
    public NetworkTables()
    {
        NtCoreInterop.NT_CreateInstance();
    }

    public void ConnectToServerV3(UIntPtr teamNumber, UIntPtr id)
    {
        //Debug.Log(id);
        //IntPtr identityPtr = CreateWPIString("10.23.86.1");
        // 1. Convert the C# string to a null-terminated UTF8 byte array
        // UTF8 is used for modern WPILib strings; ANSI/LPStr might also work
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes("dt");

        // 2. Allocate unmanaged memory for the string data
        IntPtr nativeStrPtr = Marshal.AllocHGlobal(bytes.Length + 1); // +1 for null terminator
        
        // 3. Copy the string bytes and the null terminator to unmanaged memory
        Marshal.Copy(bytes, 0, nativeStrPtr, bytes.Length);
        Marshal.WriteByte(nativeStrPtr, bytes.Length, 0); // Write null terminator

        // 4. Create the WPI_String struct with the pointer and length
        WPI_String s = new WPI_String
        {
            str = nativeStrPtr,
            len = (UIntPtr)bytes.Length // Note: NT3 often ignores len, but set it correctly
        };

        try
        {
            NtCoreInterop.NT_StartClient3(id, ref s);
        }
        finally
        {
            // nothing lol
        }
        
        NtCoreInterop.NT_SetServerTeam(id, teamNumber, defaultNT3Port);
    }

    public void Disconnect(UIntPtr id)
    {
        NtCoreInterop.NT_Disconnect(id);
    }

    public bool IsConnectedV3(UIntPtr id)
    {
        return NtCoreInterop.NT_IsConnected(id);
    }
}