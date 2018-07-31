using System;
using System.IO;
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
        static bool s_initialized;

        OpenVDBStreamDescriptor m_streamDesc;
        GameObject m_go;
        OpenVDBVolume m_volume;

        public OpenVDBStreamDescriptor streamDescriptor { get { return m_streamDesc; } }
        public GameObject gameObject { get { return m_go; } }
        public Texture3D texture3D { get { return m_volume.texture3D; } }
        public Mesh mesh { get { return m_volume.mesh; }}

        public OpenVDBStream(GameObject go, OpenVDBStreamDescriptor streamDesc)
        {
            m_go = go;
            m_streamDesc = streamDesc;
        }

        public void Dispose()
        {
            OpenVDBStream.s_streams.Remove(this);
        }

        public bool Load()
        {
            if(!s_initialized)
            {
                OpenVDBAPI.oiInitialize();
                s_initialized = true;
            }
            var context = oiContext.Create(m_go.GetInstanceID());
            var settings = m_streamDesc.settings;

            oiConfig config = new oiConfig();
            config.SetDefaults();
            config.scaleFactor = settings.scaleFactor;

            context.SetConfig(ref config);
            var path = Path.Combine(Application.streamingAssetsPath, m_streamDesc.pathToVDB);
            var loaded = context.Load(path);
            if(loaded)
            {
                UpdateVDB(context, config);
                OpenVDBStream.s_streams.Add(this);
            }
            else
            {
                Debug.LogError("failed to load openvdb at " + path);
                return false;
            }
            return true;
        }

        void UpdateVDB(oiContext context, oiConfig config)
        {
            m_volume = new OpenVDBVolume(context.volume, config);
            if(m_volume != null)
            {
                m_volume.SyncDataBegin();
                //m_volume.SyncDataEnd();
            }
        }
    }
}
