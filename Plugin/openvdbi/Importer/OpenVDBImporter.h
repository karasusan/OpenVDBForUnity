#pragma once

class oiContext;
class oiVolume;

struct oiVolumeSummary
{
    int voxel_count = 0;
    int width = 0;
    int height = 0;
    int depth = 0;

    oiVolumeSummary() {}
    oiVolumeSummary(int c, int w, int h, int d) : voxel_count(c), width(w), height(h), depth(d){}

};

struct oiVolumeData
{
    openvdbV4 *voxels = nullptr;
};

openvdbiAPI void oiInitialize();
openvdbiAPI void oiUninitialize();

openvdbiAPI oiContext* oiContextCreate(int uid);
openvdbiAPI void oiContextDestroy(oiContext* ctx);
openvdbiAPI bool oiContextLoad(oiContext* ctx, const char *path);
openvdbiAPI oiVolume* oiContextGetVolume(oiContext* ctx);

openvdbiAPI void oiVolumeGetSummary(oiVolume* volume, oiVolumeSummary* dst);
openvdbiAPI void oiVolumeFillData(oiVolume* volume, oiVolumeData* dst);