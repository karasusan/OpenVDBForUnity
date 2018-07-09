#include "pch.h"
#include "oiInternal.h"

openvdbiAPI void oiInitialize()
{
	openvdb::initialize();
}

openvdbiAPI void oiUnInitialize()
{
	openvdb::uninitialize();
}