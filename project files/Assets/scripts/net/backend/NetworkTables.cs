using System;
using System.Runtime.InteropServices;
using System.Text;
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

[StructLayout(LayoutKind.Sequential)]
public struct NT_Raw
{
    // const uint8_t*
    public IntPtr data; 
    // size_t
    public UIntPtr len; 
}

[StructLayout(LayoutKind.Sequential)]
public struct NT_Value
{
    // Field 1: The type of the data (NT_Type is an enum/int)
    public int type;

    public ulong last_change;
    public ulong server_time;

    // Field 3: The Union (contains the actual value)
    public NT_ValueData data; 
}

[StructLayout(LayoutKind.Explicit)]
public struct NT_ValueData
{
    // Boolean value (C boolean is typically marshaled as int)
    [FieldOffset(0)] 
    public int v_boolean;

    // 64-bit Integer value (int64_t maps to long, 8 bytes)
    [FieldOffset(0)]
    public long v_int64;

    // Float value (float, 4 bytes)
    [FieldOffset(0)]
    public float v_float;

    // Double value (double, 8 bytes)
    [FieldOffset(0)]
    public double v_double;

    // String value (NT_String, 16 bytes)
    [FieldOffset(0)]
    public WPI_String v_string;

    // Raw value (NT_Raw, 16 bytes)
    [FieldOffset(0)]
    public NT_Raw v_raw;
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
        WPI_String s = makeWPIString("dt");

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

    WPI_String makeWPIString(string input)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(input);

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

        return s;
    }

    public IntPtr GetEntry(UIntPtr id, string name)
    {
        WPI_String s = makeWPIString(name);

        return NtCoreInterop.NT_GetEntry(id, ref s);
    }

    public string GetName(IntPtr entry)
    {
        WPI_String s = new WPI_String();

        NtCoreInterop.NT_GetEntryName(entry, ref s);
        
        return Marshal.PtrToStringAnsi(s.str, (int)s.len);
    }
    
    public string GetString(IntPtr entry)
    {
        NT_Value value = new NT_Value();
        NtCoreInterop.NT_GetEntryValue(entry, out value);
        //Debug.Log(value.type);
        string s =  "";

        try
        {
            if (value.data.v_raw.data != IntPtr.Zero && value.data.v_raw.len.ToUInt64() != 0)
            {
                //Debug.Log("GOT VAL");
                s = Marshal.PtrToStringUTF8(value.data.v_raw.data, (int)value.data.v_raw.len);
            }
        }
        catch
        {
            
        }
        
        return s;
    }

    // VERY MUCH UNTESTED
    public void SetString(IntPtr entry, string newString)
    {
        NT_Value value = new NT_Value();
        value.type = 4; // maybe wrong
        
        value.data.v_string  = makeWPIString(newString);

        try
        {
            NtCoreInterop.NT_SetEntryValue(entry, value);
        }
        catch
        {
            
        }
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