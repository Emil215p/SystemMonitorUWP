# SystemMonitorUWP

This is a UWP applications that provides basic system information as well as monitoring(soon) of CPU, GPU, RAM, Disk.

It features 3 projects: Main UWP app, a C++ Console application for monitoring and a WAPP for bundling.

# Systems Requirements

Windows 10 1809 (17763) or above.

ARM, ARM64, x86, x64 architectures are supported.

Windows 10/11 Home/Pro are supported on x86/x64/ARM64 editions.

Windows 10 IoT Core is supported on ARM.
(I plan on updating the code to support all 4 archs on both supported editions.)

Windows 10 Mobile and Xbox is unsupported.

# Building

You will need Visual Studio 2022 or later with the UWP and C++ workloads installed.

Currently 25931 is targetted for the Windows SDK, this is unneeded, if it asks you to retarget you should pick 22621.

Note: Debugging for ARM is unsupported on Visual Studio 2022 17.14 and above. I recommend using 17.12 LTSC or 17.13 for that if ARM debugging is needed.
Building for ARM still works on Visual Studio 2022 17.14+ and Visual Studio 2026.

# More

Sorry for the bad documentation, i plan on writing more later.