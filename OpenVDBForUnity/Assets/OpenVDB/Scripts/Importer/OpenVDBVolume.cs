using System;
using UnityEngine;

namespace OpenVDB
{
    public class OpenVDBVolume: IDisposable
    {
        Texture3D m_texture;
        Mesh m_mesh;
        PinnedList<Color> m_voxels = new PinnedList<Color>();

        oiVolume m_volume;
        oiVolumeSummary m_summary;

        public Texture3D texture3D { get { return m_texture; } }

        public Mesh mesh { get { return m_mesh; } }

        public OpenVDBVolume(oiVolume volume)
        {
            m_volume = volume;

            m_volume.GetSummary(ref m_summary);
        }

        public void SyncDataBegin()
        {
            if (m_texture != null)
            {
                UnityEngine.Object.Destroy(m_texture);
                m_texture = null;
            }
            if (m_mesh != null)
            {
                UnityEngine.Object.Destroy(m_mesh);
                m_mesh = null;
            }

            // create 3d texture 
            var width = m_summary.width;
            var height = m_summary.height;
            var depth = m_summary.depth;
            m_texture = new Texture3D(width, height, depth, TextureFormat.RGBA32, false);

            // instantiate volumeData
            var volumeData = default(oiVolumeData);
            volumeData.voxels = new PinnedList<Color>(m_texture.GetPixels());

            // kick async copy
            m_volume.FillData(ref volumeData);
        }

        public void SyncDataEnd()
        {
        }

        public void Dispose()
        {
            if(m_texture != null)
            {
                UnityEngine.Object.Destroy(m_texture);
                m_texture = null;
            }
        }
    }

}
