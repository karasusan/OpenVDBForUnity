#include "pch.h"
#include "oiInternal.h"
#include "oiContext.h"
#include "oiObject.h"

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

openvdbiAPI oiObject* oiContextGetObject(oiContext* ctx)
{
    return ctx ? ctx->getObject() : 0;
}