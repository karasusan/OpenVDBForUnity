using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace OpenVDB
{
    public class OpenVDBStream : IDisposable
    {
        static List<OpenVDBStream> s_streams = new List<OpenVDBStream>();

        public OpenVDBStream(OpenVDBStreamDescriptor streamDesc)
        {
        }

        public void Dispose()
        {
            OpenVDBStream.s_streams.Remove(this);
        }
    }
}