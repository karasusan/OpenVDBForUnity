#include "pch.h"
#include "oiInternal.h"
#include "oiContext.h"
#include "oiVolume.h"
#include "OpenVDBImporter.h"

openvdbiAPI void oiInitialize()
{
	openvdb::initialize();
}

openvdbiAPI void oiUninitialize()
{
	openvdb::uninitialize();
}

openvdbiAPI oiContext* oiContextCreate(int uid)
{
    return oiContextManager::getContext(uid);
}

openvdbiAPI void oiContextDestroy(oiContext* ctx)
{
	if (ctx)
		oiContextManager::destroyContext(ctx->getUid());
}

openvdbiAPI bool oiContextLoad(oiContext* ctx, const char *path)
{
	return ctx ? ctx->load(path) : false;
}

openvdbiAPI oiVolume* oiContextGetVolume(oiContext* ctx)
{
    return ctx ? ctx->getVolume() : 0;
}

openvdbiAPI void oiVolumeGetSummary(oiVolume* volume, oiVolumeSummary* dst)
{
    if(!volume)
    {
        return;
    }
    *dst = volume->getSummary();
}

openvdbiAPI void oiVolumeFillData(oiVolume* volume, oiVolumeData* dst)
{
    if(!volume)
    {
        return;
    }
    volume->fillTextureBuffer(*dst);

}