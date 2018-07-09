#include <cstdio>
#include "openvdbi.h"

int main(int argc, char *argv[])
{
    oiInitialize();
    oiUninitialize();
    puts("Test successful\n");
    return 0;
}
