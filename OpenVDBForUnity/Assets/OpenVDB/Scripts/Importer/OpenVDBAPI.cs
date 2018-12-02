using System;
using System.Runtime.InteropServices;

namespace OpenVDB
{

    public struct oiConfig
    {
        public float scaleFactor;
        public int textureMaxSize;

        public void SetDefaults()
        {
            scaleFactor = 0.01f;
            textureMaxSize = 0;
        }
    }


    public struct oiVolumeSummary
    {
        public int voxelCount;
        public int width;
        public int height;
        public int depth;
        public int format;
        public float minValue;
        public float maxValue;
        public float xscale;
        public float yscale;
        public float zscale;
    }

    public struct oiVolumeData
    {
        public IntPtr voxels;
    }

    public struct oiContext
    {
        public IntPtr self;
        public static implicit operator bool(oiContext v) { return v.self != IntPtr.Zero; }

        public static oiContext Create(int uid) { return oiContextCreate(uid); }
        public bool Load(string path) { return oiContextLoad(self, path); }
        public void SetConfig(ref oiConfig conf) { oiContextSetConfig(self, ref conf); }
        public void Destroy() { oiContextDestroy(self); self = IntPtr.Zero; }
        public oiVolume volume { get { return oiContextGetVolume(self); } }

        #region internal
        [DllImport("openvdbi")] static extern oiContext oiContextCreate(int uid);
        [DllImport("openvdbi")] static extern Bool oiContextLoad(IntPtr ctx, string path);
        [DllImport("openvdbi")] static extern void oiContextSetConfig(IntPtr ctx, ref oiConfig conf);
        [DllImport("openvdbi")] static extern void oiContextDestroy(IntPtr ctx);
        [DllImport("openvdbi")] static extern oiVolume oiContextGetVolume(IntPtr ctx);
        #endregion
    }

    public struct oiVolume
    {
        public IntPtr self;
        public static implicit operator bool(oiVolume v) { return v.self != IntPtr.Zero; }
        public void GetSummary(ref oiVolumeSummary dst) { oiVolumeGetSummary(self, ref dst); }
        public void FillData(ref oiVolumeData dst) { oiVolumeFillData(self, ref dst); }

        #region internal
        [DllImport("openvdbi")] static extern void oiVolumeGetSummary(IntPtr volume, ref oiVolumeSummary dst);
        [DllImport("openvdbi")] static extern void oiVolumeFillData(IntPtr volume, ref oiVolumeData dst);
        #endregion
    }

    public static class OpenVDBAPI
    {
        [DllImport("openvdbi")] public static extern void oiInitialize();
        [DllImport("openvdbi")] public static extern void oiUninitialize();
    }
}
