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

public readonly struct NT_Entry
{
    public readonly IntPtr Handle;
    public NT_Entry(IntPtr handle) => Handle = handle;
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
    
    public string GetString(IntPtr entryHandle, string defaultValue = "")
    {
        WPI_String ntString = new WPI_String();
        string managedString = defaultValue;

        WPI_String dVal = makeWPIString(defaultValue);

        try
        {
            ntString = NtCoreInterop.NT_GetString(entryHandle);

            // 2. Check if the pointer is valid (not the default value string pointer)
            if (ntString.str != IntPtr.Zero && (ulong)ntString.len > 0)
            {
                // 3. Marshal the native UTF-8 string pointer into a managed C# string
                
                // We need to determine the correct byte length of the string.
                // The 'len' parameter (UIntPtr) provides the byte count.
                int byteLength = (int)(ulong)ntString.len;

                // Marshal.PtrToStringUTF8 is often available in newer .NET versions,
                // but the manual way below is more compatible and handles the length precisely
                // based on what NT_GetString provided.
                
                byte[] buffer = new byte[byteLength];
                Marshal.Copy(ntString.str, buffer, 0, byteLength);
                managedString = Encoding.UTF8.GetString(buffer);
            }
            else
            {
                // If nativeStrPtr is IntPtr.Zero or length is 0, the function likely 
                // returned the default value (which is not a pointer to be freed).
                managedString = defaultValue;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving string from NTcore: {ex.Message}");
            managedString = defaultValue;
        }
        finally
        {
            if (ntString.str != null)
            {
                // 4. CRITICAL: Free the native memory if the function returned a pointer.
                // We must only free memory allocated by NT_GetString (i.e., if it returned a pointer 
                // that is not null and is not pointing to the internal default value buffer).
                // A correct NTcore implementation will require a free if the pointer is non-zero.
                if (ntString.str != IntPtr.Zero)
                {
                    NtCoreInterop.NT_DisposeString(ntString.str);
                }
            }
        }
        
        return managedString;
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