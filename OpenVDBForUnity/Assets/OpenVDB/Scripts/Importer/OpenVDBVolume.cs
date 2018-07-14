using System;
using UnityEngine;

namespace OpenVDB
{
    public class OpenVDBVolume: IDisposable
    {
        Texture3D m_texture3D;
        Mesh m_mesh;

        oiVolume m_volume;
        oiVolumeSummary m_summary;

        public Texture3D texture3D { get { return m_texture3D; } }

        public Mesh mesh { get { return m_mesh; } }

        public OpenVDBVolume(oiVolume volume)
        {
            m_volume = volume;

            m_volume.GetSummary(ref m_summary);
        }

        public void SyncDataBegin()
        {
            if (m_texture3D != null)
            {
                UnityEngine.Object.Destroy(m_texture3D);
                m_texture3D = null;
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
            var format = (TextureFormat)m_summary.format;
            m_texture3D = new Texture3D(width, height, depth, format, false);

            // instantiate volumeData
            var volumeData = default(oiVolumeData);
            volumeData.voxels = new PinnedList<Color>(m_texture3D.GetPixels());

            // kick async copy
            m_volume.FillData(ref volumeData);

            // create mesh
            var position = Vector3.zero;
            m_mesh = Voxelizer.VoxelMesh.Build(new []{position}, 1f);
        }

        public void SyncDataEnd()
        {
        }

        public void Dispose()
        {
            if(m_texture3D != null)
            {
                UnityEngine.Object.Destroy(m_texture3D);
                m_texture3D = null;
            }
        }
    }

}
