#include "pch.h"
#include "oiInternal.h"
#include "oiContext.h"
#include "oiObject.h"

oiObject::oiObject()
{
}

oiObject::~oiObject()
{
}

oiContext*  oiObject::getContext() const { return m_ctx; }

//const char* oiObject::getName() const { return m_abc.getName().c_str(); }
