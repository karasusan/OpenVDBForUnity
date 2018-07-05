using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace OpenVDB
{
    public static class OpenVDBAPI
    {
        [DllImport("openvdbi")] public static extern void oiInitialize();
    }
}
