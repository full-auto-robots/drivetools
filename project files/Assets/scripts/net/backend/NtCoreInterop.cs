using UnityEngine;
using System.Runtime.InteropServices;
using System;

// welcome to the interop layer! its fun here
// this code is sponsored by ChatGPT

// the point of this class is to take the ntcore.dll file (courtesy of WPI),
// and make it readable for a c# implementation

internal static class NtCoreInterop
{
    private const string DLL = "ntcore";

    // NT instance stuff ***
    [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern UIntPtr  NT_GetDefaultInstance();

    [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern UIntPtr NT_CreateInstance();


    [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern void NT_DestroyInstance();
    // ***

    // we have no need for starting a local NT

    // CLIENT STUFF ***
    [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern void NT_StartClient3(
        UIntPtr inst, // Confirmed: UIntPtr works for NT_Inst
        
        // *** NEW: Pass a reference (pointer) to the struct ***
        ref WPI_String identity 
    );

    [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern void NT_StartClient4(
        UIntPtr inst, // Confirmed: UIntPtr works for NT_Inst
        
        // *** NEW: Pass a reference (pointer) to the struct ***
        ref WPI_String identity,

        UIntPtr port
    );


    [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern void NT_StopClient(UIntPtr inst);



    [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern void NT_StartDSClient(UIntPtr inst, UIntPtr port);

    [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern void NT_StopDSClient(UIntPtr inst);
    
    // ***

    // FOR CONNECTING TO THE SERVER ***
    [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern void NT_SetServer(UIntPtr inst, ref WPI_String server_name, UIntPtr port);


    [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern void NT_SetServerTeam(UIntPtr inst, UIntPtr team, UIntPtr port);



    [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern void NT_Disconnect(UIntPtr inst);



    [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    public static extern bool NT_IsConnected(UIntPtr inst);

    // ***
}