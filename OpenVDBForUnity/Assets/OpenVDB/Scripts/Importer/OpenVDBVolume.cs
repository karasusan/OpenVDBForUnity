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
            var voxelCount = m_summary.voxelCount;
            m_voxels.ResizeDiscard(voxelCount);

            var volumeData = default(oiVolumeData);
            volumeData.voxels = m_voxels;

            // kick async copy
            m_volume.FillData(ref volumeData);
        }

        public void SyncDataEnd()
        {
            if(m_texture != null)
            {
                UnityEngine.Object.Destroy(m_texture);
                m_texture = null;
            }
            if (m_mesh != null)
            {
                UnityEngine.Object.Destroy(m_mesh);
                m_mesh = null;
            }

            var width = m_summary.width;
            var height = m_summary.height;
            var depth = m_summary.depth;
            m_texture = new Texture3D(width, height, depth, TextureFormat.RGBA32, false);
            m_texture.SetPixels(m_voxels.Array, 0);

            //mesh = 
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
