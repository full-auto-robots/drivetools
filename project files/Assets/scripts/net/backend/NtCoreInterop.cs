using UnityEngine;
using System.Runtime.InteropServices;
using System;

internal static class NtCoreInterop
{
    private const string DLL = "ntcore";

    [DllImport(DLL, EntryPoint="NT_CreateInstance", CallingConvention = CallingConvention.Cdecl)]
    public static extern int  NT_CreateInstance();
}