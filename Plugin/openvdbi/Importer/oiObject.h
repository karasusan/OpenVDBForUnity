#pragma once
class oiContext;

class oiObject {

public:
    oiObject();
    virtual ~oiObject();

    const char* getName() const;

public:
    // for internal use
    oiContext*  getContext() const;

protected:
    oiContext *m_ctx = nullptr;
};