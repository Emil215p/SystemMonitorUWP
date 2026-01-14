# SystemMonitorUWP

This is a UWP applications that provides basic system information as well as monitoring(soon) of CPU, GPU, RAM, Disk.

It features 3 projects:
The main UWP app, a C++ Console application for monitoring and a WAPP for bundling.

It features an update system so it can update itself from the app by downloading the latest release version from GitHub releases.

# Systems Requirements

Windows 10 version 1809 (17763) or above.

ARM/ARM64/x86/x64 architectures are supported.

Windows 10/11 Desktop are supported on x86/x64/ARM64 editions.

Windows 10 IoT Core is supported on ARM.
I plan on updating the code to support all 4 archs on both supported editions eventually.

# Building

You will need Visual Studio 2022 or 2026 with the UWP and C++ workloads installed.

Currently build 25931 is targetted for the Windows SDK, if it asks to retarget then picking 22621 should work fine, any version below that is untested.

Note:
Debugging for ARM is unsupported on Visual Studio 2022 17.14 and above. 
If you need ARM debugging you will need to use Visual Studio 2022 17.13 or below.
Personally i have tested 17.13 and 17.12 and they both work just fine for the purpose.