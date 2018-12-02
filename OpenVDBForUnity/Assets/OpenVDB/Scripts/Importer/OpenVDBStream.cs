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

        private GameObject m_go;
        private OpenVDBVolume m_volume;
        private OpenVDBStreamDescriptor m_streamDescriptor;
        
        public GameObject gameObject { get { return m_go; } }

        public Texture3D texture3D { get { return m_volume.texture3D; } }

        public Mesh mesh { get { return m_volume.mesh; } } 

        public OpenVDBStream(GameObject go, OpenVDBStreamDescriptor mStreamDesc)
        {
            m_go = go;
            m_streamDescriptor = mStreamDesc;
        }

        public void Dispose()
        {
            s_streams.Remove(this);
        }

        public bool Load()
        {
            if(!s_initialized)
            {
                OpenVDBAPI.oiInitialize();
                s_initialized = true;
            }
            var context = oiContext.Create(m_go.GetInstanceID());
            var settings = m_streamDescriptor.settings;

            var config = new oiConfig();
            config.SetDefaults();
            config.scaleFactor = settings.scaleFactor;
            config.textureMaxSize = settings.textureMaxSize;

            context.SetConfig(ref config);
            var path = Path.Combine(Application.streamingAssetsPath, m_streamDescriptor.pathToVDB);
            var loaded = context.Load(path);
            if(loaded)
            {
                UpdateVDB(context);
                s_streams.Add(this);
            }
            else
            {
                Debug.LogError("failed to load openvdb at " + path);
                return false;
            }
            return true;
        }

        void UpdateVDB(oiContext context)
        {
            m_volume = new OpenVDBVolume(context.volume);
            m_volume.SyncDataBegin();
            m_volume.texture3D.name = m_go.name;
            
            // Apply volume scale
            m_go.transform.localScale = m_volume.scale;
        }
    }
}
