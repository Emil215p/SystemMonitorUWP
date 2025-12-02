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
#define MESSAGE_WIDTH 30
#define NUMERIC_WIDTH 10

void appendMessage(vector<string>& arr, LPCSTR msg, bool addColon) {
    ostringstream oss;
    oss.width(MESSAGE_WIDTH);
    oss << msg;
    if (addColon) oss << " : ";
    arr.push_back(oss.str());
}

void appendMessageLine(vector<string>& arr, LPCSTR msg) {
    appendMessage(arr, msg, false);
}

void appendMessageLine(vector<string>& arr, LPCSTR msg, DWORD value) {
    ostringstream oss;
    appendMessage(arr, msg, true);
    oss.width(NUMERIC_WIDTH);
    oss << right << value;
    arr.push_back(oss.str());
}

void appendMessageLine(vector<string>& arr, LPCSTR msg, DWORDLONG value) {
    ostringstream oss;
    appendMessage(arr, msg, true);
    oss.width(NUMERIC_WIDTH);
    oss << right << value;
    arr.push_back(oss.str());
}

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

int main(int argc, char** argv)
{
    vector<string> output;

    appendMessageLine(output, "Starting to monitor memory.");

    MEMORYSTATUSEX statex;
    statex.dwLength = sizeof(statex);

    BOOL success = ::GlobalMemoryStatusEx(&statex);
    if (!success) {
        DWORD error = GetLastError();
        appendMessageLine(output, "Error getting memory information", error);
    }
    else {
        DWORD load = statex.dwMemoryLoad;
        DWORDLONG physKb = statex.ullTotalPhys / DIV;
        DWORDLONG freePhysKb = statex.ullAvailPhys / DIV;
        DWORDLONG pageKb = statex.ullTotalPageFile / DIV;
        DWORDLONG freePageKb = statex.ullAvailPageFile / DIV;
        DWORDLONG virtualKb = statex.ullTotalVirtual / DIV;
        DWORDLONG freeVirtualKb = statex.ullAvailVirtual / DIV;
        DWORDLONG freeExtKb = statex.ullAvailExtendedVirtual / DIV;

        appendMessageLine(output, "Percent of memory in use", load);
        appendMessageLine(output, "KB of physical memory", physKb);
        appendMessageLine(output, "KB of free physical memory", freePhysKb);
        appendMessageLine(output, "KB of paging file", pageKb);
        appendMessageLine(output, "KB of free paging file", freePageKb);
        appendMessageLine(output, "KB of virtual memory", virtualKb);
        appendMessageLine(output, "KB of free virtual memory", freeVirtualKb);
        appendMessageLine(output, "KB of free extended memory", freeExtKb);
        appendMessageLine(output, "");
    }

    appendMessageLine(output, "No longer monitoring memory.");
	for (const auto& line : output) {
		printf("%s\n", line.c_str());
	}

    std::wstring folder = GetLocalFolderPath();
    std::wstring filePath = folder + L"\\Common.txt";
    std::ofstream file(filePath);
    if (file.is_open()) {
        file << "Hello from ConsoleApplication!" << std::endl;
        file.close();
    }

    return 0;
}