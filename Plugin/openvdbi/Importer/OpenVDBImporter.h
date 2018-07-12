#pragma once

class oiContext;
class oiObject;

openvdbiAPI void oiInitialize();
openvdbiAPI void oiUninitialize();

openvdbiAPI oiContext* oiContextCreate(int uid);
openvdbiAPI void oiContextDestroy(oiContext* ctx);
openvdbiAPI bool oiContextLoad(oiContext* ctx, const char *path);
openvdbiAPI oiObject* oiContextGetObject(oiObject* ctx);