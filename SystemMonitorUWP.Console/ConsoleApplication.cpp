#include "pch.h"
#include <windows.h>
#include <chrono>
#include <thread>
#include <vector>
#include <string>
#include <sstream>
#include <fstream>
#include <ShlObj.h>

using namespace std;

#define DIV 1024

std::wstring GetLocalFolderPath() {
    PWSTR path = nullptr;
    HRESULT hr = SHGetKnownFolderPath(FOLDERID_LocalAppData, 0, nullptr, &path);
    std::wstring result;
    if (SUCCEEDED(hr)) {
        result = path;
        result += L"\\Packages\\6ec4adb3-d04f-434d-b082-22e3e256aaa3_8ycaqhn5qd882\\LocalState";
    }
    CoTaskMemFree(path);
    return result;
}

static void GetMemoryStatus(DWORD& load, DWORDLONG& physKb, DWORDLONG& freePhysKb, DWORDLONG& pageKb, DWORDLONG& freePageKb, DWORDLONG& virtualKb, DWORDLONG& freeVirtualKb, DWORDLONG& freeExtKb)
{
    MEMORYSTATUSEX statex{};
    statex.dwLength = sizeof(statex);

    if (::GlobalMemoryStatusEx(&statex)) {
        load = statex.dwMemoryLoad;
        physKb = statex.ullTotalPhys / DIV;
        freePhysKb = statex.ullAvailPhys / DIV;
        pageKb = statex.ullTotalPageFile / DIV;
        freePageKb = statex.ullAvailPageFile / DIV;
        virtualKb = statex.ullTotalVirtual / DIV;
        freeVirtualKb = statex.ullAvailVirtual / DIV;
        freeExtKb = statex.ullAvailExtendedVirtual / DIV;
    }
}

int main(int argc, char** argv)
{
   vector<string> output;

   DWORD load = 0;
   DWORDLONG physKb = 0;
   DWORDLONG freePhysKb = 0;
   DWORDLONG pageKb = 0;
   DWORDLONG freePageKb = 0;
   DWORDLONG virtualKb = 0;
   DWORDLONG freeVirtualKb = 0;
   DWORDLONG freeExtKb = 0;

   GetMemoryStatus(load, physKb, freePhysKb, pageKb, freePageKb, virtualKb, freeVirtualKb, freeExtKb);

   std::wstring folder = GetLocalFolderPath();
   std::wstring filePath = folder + L"\\Common.csv";
   std::ofstream file(filePath);
   if (file.is_open()) {
       file << load << ",";
       file << physKb << ",";
       file << freePhysKb << ",";
       file << pageKb << ",";
       file << freePageKb << ",";
       file << virtualKb << ",";
       file << freeVirtualKb << ",";
       file << freeExtKb;
       file.close();
   }

   return 0;
}