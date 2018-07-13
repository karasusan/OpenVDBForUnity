#include <cstdio>
#include "pch.h"
#include "openvdbi.h"

int main(int argc, char *argv[])
{
    oiInitialize();

    auto uid = 1;
    oiContext* ctx = oiContextCreate(uid);
    auto succeed = oiContextLoad(ctx, "");
    oiContextDestroy(ctx);

    printf("%d\n", succeed);

    oiUninitialize();

    puts("Test successful\n");
    return 0;
}
